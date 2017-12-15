using JetBrains.Annotations;
using NewRelic.Agent.Core.ThreadProfiling;
using NewRelic.Agent.Core.Tracer;
using System;

namespace NewRelic.Agent.Core
{
	/// <summary>
	/// This is the main handle to and agent.  There is only one agent per System.AppDomain.
	/// </summary>
	public interface IAgent
	{
		[NotNull]
		ThreadProfilingService ThreadProfilingService { get; }

		/// <summary>
		/// Gets an ITracer to manage the trace information across the call.
		/// An ITracer is created by a TracerFactory,
		/// as invoked from injected byte code placed at the start of each traced function.
		/// </summary>
		/// <param name="tracerFactoryName"></param>
		/// <param name="tracerArguments">A packed value with items from the instrumentation .xml files</param>
		/// <param name="metricName"></param>
		/// <param name="assemblyName"></param>
		/// <param name="type"></param>
		/// <param name="typeName"></param>
		/// <param name="methodName"></param>
		/// <param name="argumentSignature"></param>
		/// <param name="invocationTarget"></param>
		/// <param name="arguments"></param>
		/// <returns>Returns an ITracer as an Object, since that built-in type is much easier to use in call-point type signatures</returns>
		ITracer GetTracerImpl(String tracerFactoryName, UInt32 tracerArguments, String metricName, [NotNull] String assemblyName, [NotNull] Type type, [NotNull] String typeName, [NotNull] String methodName, [NotNull] String argumentSignature, Object invocationTarget, [NotNull] Object[] arguments, UInt64 functionId);

		/// <summary>
		/// The agent's current state.  It manages an orderly startup sequence.
		/// </summary>
	    AgentState State { get; }
	}
}
 
