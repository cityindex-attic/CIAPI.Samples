using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CIAPI.DTO;
using Common.Logging;

namespace CiapiNewsTest
{
	class Program
	{
		private static readonly Uri RPC_URI = new Uri("https://ciapipreprod.cityindextest9.co.uk/TradingApi");
		private static readonly Uri STREAMING_URI = new Uri("https://pushpreprod.cityindextest9.co.uk");
		private const string USERNAME = "xx189949";
		private const string PASSWORD = "password";

		static void Main(string[] args)
		{
			try
			{
				var listeners = new[] { new TextWriterTraceListener(Console.Out) };
				Debug.Listeners.AddRange(listeners);

				var adapter = new MyLoggerFactoryAdapter(null) { OnMessage = AddLogMessage };
				LogManager.Adapter = adapter;

				var client = new CIAPI.Rpc.Client(RPC_URI);
				client.LogIn(USERNAME, PASSWORD);

				var news = client.News.ListNewsHeadlinesWithSource("mni", "ALL", 10);

				client.LogOut();
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc);
			}
		}

		static void AddLogMessage(LogLevel level, object message, Exception exception)
		{
			AddLogMessage(message, level);
			AddLogMessage(exception, LogLevel.Error);
		}

		static void AddLogMessage(object val, LogLevel level)
		{
			if (val == null)
				return;

			if (val is Exception)
			{
				Console.WriteLine(val);
				Trace.WriteLine(val);
			}

			var text = val.ToString() + Environment.NewLine;

			Trace.WriteLine(string.Format("{0} {1}", DateTime.UtcNow, text));
		}
	}
}
