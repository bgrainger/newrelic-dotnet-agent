﻿using System;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NewRelic.Agent.Extensions.Providers.Wrapper;

namespace NewRelic.Providers.Wrapper.MongoDb
{
	public static class MongoDBHelper
	{
		public static String GetCollectionModelName(MethodCall methodCall)
		{
			var collection = methodCall.InvocationTarget as MongoCollection;
			if (collection == null)
				throw new Exception("Method's invocation target is not a MongoCollection.");
			return collection.Name;
		}
	}
}
