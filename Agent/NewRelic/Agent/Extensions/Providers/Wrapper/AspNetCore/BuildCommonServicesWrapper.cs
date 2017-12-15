﻿using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NewRelic.Agent.Extensions.Providers.Wrapper;

namespace NewRelic.Providers.Wrapper.AspNetCore
{
	public class BuildCommonServicesWrapper : IWrapper
	{
		public bool IsTransactionRequired => false;

		public CanWrapResponse CanWrap(InstrumentedMethodInfo methodInfo)
		{
			return new CanWrapResponse("BuildCommonServicesWrapper".Equals(methodInfo.RequestedWrapperName));
		}

		public AfterWrappedMethodDelegate BeforeWrappedMethod(InstrumentedMethodCall instrumentedMethodCall, IAgentWrapperApi agentWrapperApi, ITransaction transaction)
		{
			//Do nothing at start of method

			return Delegates.GetDelegateFor<IServiceCollection>(onSuccess: HandleSuccess);

			void HandleSuccess(IServiceCollection services)
			{
				//Forced evaluation is important. Do not remove ToList()
				var startupFilters = services.Where(serviceDescriptor => serviceDescriptor.ServiceType == typeof(IStartupFilter)).ToList();
				
				services.AddTransient<IStartupFilter>(provider => new AddNewRelicStartupFilter(agentWrapperApi));
				
				foreach (var filter in startupFilters)
				{
					services.Remove(filter); // Remove from early in pipeline
					services.Add(filter); // Add to end after our AddNewRelicStartupFilter
				}
			}
		}
	}
}
