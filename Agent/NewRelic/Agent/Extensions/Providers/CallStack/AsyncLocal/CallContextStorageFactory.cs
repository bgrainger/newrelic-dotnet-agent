﻿using NewRelic.Agent.Extensions.Providers;

namespace NewRelic.Providers.CallStack.AsyncLocal
{
	public class CallContextStorageFactory : IContextStorageFactory
	{
		//Not searching inheritance tree for now to avoid any additional perf penalty
		private const bool ShouldSearchParentsForAttribute = false; 

		public bool IsAsyncStorage => true;
		public bool IsValid => true;
		public ContextStorageType Type => ContextStorageType.CallContextLogicalData;

		public IContextStorage<T> CreateContext<T>(string key)
		{
			if(TypeNeedsSerializableContainer<T>())
			{
				return new CallContextWrappedStorage<T>(key);
			}
			else
			{
				return new CallContextStorage<T>(key);
			}
		}

		private static bool TypeNeedsSerializableContainer<T>()
		{
			return typeof(T).IsDefined(typeof(NeedSerializableContainer), ShouldSearchParentsForAttribute);
		}
	}
}
