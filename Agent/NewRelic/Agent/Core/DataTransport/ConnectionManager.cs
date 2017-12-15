﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using JetBrains.Annotations;
using NewRelic.Agent.Core.Logging;
using NewRelic.Agent.Core.Events;
using NewRelic.Agent.Core.Exceptions;
using NewRelic.Agent.Core.Time;
using NewRelic.Agent.Core.Utilities;
using NewRelic.SystemExtensions;

namespace NewRelic.Agent.Core.DataTransport
{
	/// <summary>
	/// The <see cref="ConnectionManager"/> understands the business logic of *when* to connect, disconnect, send data, etc. It is the companion of <see cref="ConnectionHandler"/> which knows *how* to connect, disconnect, etc.
	/// 
	/// The main purpose of the <see cref="ConnectionManager"/> is to ensure that <see cref="ConnectionHandler"/> is used a thread-safe manner. It also listens for events such as `RestartAgentEvent` to trigger reconnects. All calls into <see cref="ConnectionHandler"/> are synchronized with locks.
	/// </summary>
	public class ConnectionManager : ConfigurationBasedService, IConnectionManager
	{
		private static readonly TimeSpan MinimumRetryTime = TimeSpan.FromSeconds(5);
		private static readonly TimeSpan MaximumRetryTime = TimeSpan.FromMinutes(5);

		[NotNull]
		private readonly IConnectionHandler _connectionHandler;

		[NotNull]
		private readonly IScheduler _scheduler;

		private TimeSpan _retryTime = MinimumRetryTime;

		private Boolean _started;

		[NotNull]
		private readonly Object _syncObject = new Object();

		public ConnectionManager([NotNull] IConnectionHandler connectionHandler, [NotNull] IScheduler scheduler)
		{
			_connectionHandler = connectionHandler;
			_scheduler = scheduler;

			_subscriptions.Add<StartAgentEvent>(OnStartAgent);
			_subscriptions.Add<RestartAgentEvent>(OnRestartAgent);

			// calling Disconnect on Shutdown is crashing on Linux.  This is probably a CLR bug, but we have to work around it.
			// The Shutdown call is actually not very important (agent runs time out after 5 minutes anyway) so just don't call it.
#if NET35
			_subscriptions.Add<CleanShutdownEvent>(OnCleanShutdown);
#endif

			if (_configuration.AutoStartAgent)
				Start();
		}

		#region Synchronized methods

		private void Start()
		{
			// First, a quick happy path check which won't force callers to wait around to find out that we've already started a long time ago
			if (_started)
				return;

			lock (_syncObject)
			{
				// Second, a thread-safe check inside the blocking code block that ensures we'll never start more than once
				if (_started)
					return;

				if (_configuration.CollectorSyncStartup || _configuration.CollectorSendDataOnExit)
					Connect();
				else
					_scheduler.ExecuteOnce(Connect, TimeSpan.Zero);

				_started = true;
			}
		}

		private void Connect()
		{
			try
			{
				lock (_syncObject)
				{
					_connectionHandler.Connect();
				}

				_retryTime = MinimumRetryTime;
			}
			// This exception is thrown when the Agent is to restart; for example when Agent settings on the APM change.
			catch (ForceRestartException)
			{
				ScheduleRestart();
			}
			// This exception is thrown when there has been a connection error between the collector(APM) and the Agent.
			catch (ConnectionException)
			{
				ScheduleRestart();
			}
			//This exception is thrown when the agent receives a service unavailable error
			catch (ServiceUnavailableException)
			{
				ScheduleRestart();
			}
			// Occurs when the agent connects to APM but the connection gets aborted by the collector
			catch (SocketException)
			{
				ScheduleRestart();
			}
			// Occurs when the agent is unable to read data from the transport connection (this might occur when a socket exception happens - in that case the exception will be caught above)
			catch (IOException)
			{
				ScheduleRestart();
			}
			// Occurs when no network connection is available, DNS unavailable, etc.
			catch (WebException)
			{
				ScheduleRestart();
			}
			// This catch all is in place so that we avoid doing harm for all of the potentially destructive things that could happen during a connect.
			// We want to error on the side of doing no harm to our customers
			catch (Exception ex)
			{
				ImmediateShutdown(ex.Message);
			}
		}

		private void Disconnect()
		{
			lock (_syncObject)
			{
				_connectionHandler.Disconnect();
			}
		}

		private void Reconnect()
		{
			lock (_syncObject)
			{
				Disconnect();
				Connect();
			}
		}

		public T SendDataRequest<T>(String method, params Object[] data)
		{
			lock (_syncObject)
			{
				return _connectionHandler.SendDataRequest<T>(method, data);
			}
		}

		#endregion Synchronized methods

		#region Helper methods

		private static void ImmediateShutdown(String message)
		{
			Log.InfoFormat("Shutting down: {0}", message);
			EventBus<KillAgentEvent>.Publish(new KillAgentEvent());
		}

		private void ScheduleRestart()
		{
			Log.InfoFormat("Will attempt to reconnect in {0} seconds", _retryTime.TotalSeconds);
			_scheduler.ExecuteOnce(Connect, _retryTime);

			_retryTime = TimeSpanMath.Min(_retryTime.Multiply(2), MaximumRetryTime);
		}

		#endregion

		#region Event handlers

		protected override void OnConfigurationUpdated(ConfigurationUpdateSource configurationUpdateSource)
		{
			// If we receive a non-server config update while connected then we need to reconnect.
			// Receiving a server config update implies that we just connected or disconnected so there's no need to do anything.
			if (configurationUpdateSource == ConfigurationUpdateSource.Server)
				return;
			if (_configuration.AgentRunId == null)
				return;

			Log.Info("Reconnecting due to configuration change");

			_scheduler.ExecuteOnce(Reconnect, TimeSpan.Zero);
		}

		private void OnStartAgent(StartAgentEvent eventData)
		{
			Start();
		}

		private void OnRestartAgent(RestartAgentEvent eventData)
		{
			Connect();
		}

		private void OnCleanShutdown(CleanShutdownEvent eventData)
		{
			Disconnect();
		}

		#endregion Event handlers
	}
}
