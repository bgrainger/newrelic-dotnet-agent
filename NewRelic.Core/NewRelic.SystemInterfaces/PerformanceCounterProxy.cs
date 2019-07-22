﻿#if NETFRAMEWORK
using NewRelic.Core.Logging;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

namespace NewRelic.SystemInterfaces
{
	public interface IPerformanceCounterProxy : IDisposable
	{
		float NextValue();
	}

	public class PerformanceCounterProxy : IPerformanceCounterProxy
	{
		private readonly PerformanceCounter _counter;

		public PerformanceCounterProxy(string categoryName, string counterName, string instanceName)
		{
			_counter = new PerformanceCounter(categoryName, counterName, instanceName);
		}

		public float NextValue()
		{
			return _counter.NextValue();
		}
		
		public void Dispose()
		{
			_counter?.Dispose();
		}
	}

	public interface IPerformanceCounterCategoryProxy
	{
		string[] GetInstanceNames();
	}

	public class PerformanceCounterCategoryProxy : IPerformanceCounterCategoryProxy
	{
		private readonly PerformanceCounterCategory _performanceCounterCategory;

		public PerformanceCounterCategoryProxy(string categoryName)
		{
			_performanceCounterCategory = new PerformanceCounterCategory(categoryName);
		}

		public string[] GetInstanceNames()
		{
			return _performanceCounterCategory.GetInstanceNames();
		}
	}

	public interface IPerformanceCounterProxyFactory
	{
		IPerformanceCounterProxy CreatePerformanceCounterProxy(string categoryName, string counterName);
	}

	public class PerformanceCounterProxyFactory : IPerformanceCounterProxyFactory
	{
		public const string ProcessIdCounterName = "Process ID";

		private readonly IProcessStatic _processStatic;

		/// <summary>
		/// Function used to create the performance counter category which is used to list the
		/// instance names of the processes that are reporting information for that performance counter
		/// category.  This is dependency injected so that we can manipulate its behavior for testing purposes
		/// </summary>
		private readonly Func<string, IPerformanceCounterCategoryProxy> _createPerformanceCounterCategory;

		/// <summary>
		/// Function used to create the actual performance counter proxy.
		/// This is dependency injected so that we can manipulate its behavior for testing purposes
		/// Func params: categoryName, perfCounterName, instanceName
		/// </summary>
		private readonly Func<string, string, string, IPerformanceCounterProxy> _createPerformanceCounter;

		//Dictionary that relates the category to the instance name of the current process for that category.
		private static readonly ConcurrentDictionary<string, string> _currentProcessInstanceNames = new ConcurrentDictionary<string, string>();

		public PerformanceCounterProxyFactory(IProcessStatic processStatic, Func<string, IPerformanceCounterCategoryProxy> performanceCounterCategoryProxyFactory, Func<string, string, string, IPerformanceCounterProxy> performanceCounterCreator)
		{
			_processStatic = processStatic;
			_createPerformanceCounterCategory = performanceCounterCategoryProxyFactory;
			_createPerformanceCounter = performanceCounterCreator;
		}

		/// <summary>
		/// Used by dependency injection to resolve the factory for creating a perf counter category proxy
		/// </summary>
		/// <param name="categoryName"></param>
		/// <returns></returns>
		public static IPerformanceCounterCategoryProxy DefaultCreatePerformanceCounterCategoryProxy(string categoryName)
		{
			return new PerformanceCounterCategoryProxy(categoryName);
		}

		/// <summary>
		/// Used by dependency injection to resolve the factory for creating a perf counter
		/// </summary>
		/// <param name="categoryName"></param>
		/// <returns></returns>
		public static IPerformanceCounterProxy DefaultCreatePerformanceCounterProxy(string categoryName, string counterName, string instanceName)
		{
			return new PerformanceCounterProxy(categoryName, counterName, instanceName);
		}

		/// <summary>
		/// Responsible for obtaining the instance name for the current process for a perf counter
		/// category.
		/// </summary>
		/// <returns>the instance name or NULL if one could not be identified.</returns>
		/// <param name="categoryName"></param>
		private string GetCurrentProcessInstanceNameForCategory(string categoryName)
		{
			var processName = _processStatic.GetCurrentProcess().ProcessName;
			var pid = _processStatic.GetCurrentProcess().Id;

			var result = GetInstanceNameForProcessAndCategory(categoryName, processName, pid);

			return result;
		}

		public IPerformanceCounterProxy CreatePerformanceCounterProxy(string categoryName, string counterName)
		{
			var instanceName = _currentProcessInstanceNames.GetOrAdd(categoryName, GetCurrentProcessInstanceNameForCategory);

			if (instanceName == null)
			{
				throw new Exception($"Unable to obtain current process' instance name for category {categoryName} when trying to build counter {counterName}.");
			}

			return _createPerformanceCounter(categoryName, counterName, instanceName);
		}

		/// <summary>
		/// The instance name of a performance counter is a combination of the process Name and an index id to 
		/// uniquely identify it when multiple of the same process are running.  For example "TestApp #1" pid=82, "TestApp #2" pid=134
		/// are two instances of the same executable, TestApp.  In order to determine if an instance matches a process,
		/// an additional performance counter "Process ID" must be used which returns the PID of the process.
		/// </summary>
		/// <param name="perfCategoryName"></param>
		/// <param name="processName"></param>
		/// <param name="pid"></param>
		/// <returns>The instance name that will be used to collect performance counter data for the process</returns>
		private string GetInstanceNameForProcessAndCategory(string perfCategoryName, string processName, int pid)
		{
			var performanceCategory = _createPerformanceCounterCategory(perfCategoryName);

			var instanceNames = performanceCategory.GetInstanceNames()
				.Where(x => x.StartsWith(processName, StringComparison.OrdinalIgnoreCase));

			foreach (var instanceName in instanceNames)
			{
				try
				{
					using (var pcPid = _createPerformanceCounter(perfCategoryName, ProcessIdCounterName, instanceName))
					{
						if ((int)pcPid.NextValue() == pid)
						{
							return instanceName;
						}
					}
				}
				catch (Exception ex)
				{
					//Log a message here and continue because this instance may not be relevant to the process that we are looking for.
					Log.Finest($"Error determining if instance '{instanceName}' is process '{processName}'({pid}). Error: {ex}");
				}
			}

			//At this point a match was not found. The caller should handle accordingly
			return null;
		}
	}
}
#endif