using System;
using System.Reflection;
using System.Linq;
using CIAPI.Rpc;
using CIAPI.Streaming;
using System.Collections.Generic;
using CIAPI.StreamingClient;
using System.Configuration;
using System.Threading;

namespace STREAMINGALL_adapater
{
	class MainClass
	{
		public static List<IStreamingListener> _listeners = new List<IStreamingListener> ();
		public static IStreamingClient _streamingClient;
		public static Client _rpcClient;

		public static void Main (string[] args)
		{
			try 
			{
				LogAssembliesBeingUsed ();
				Connect();

				Console.WriteLine ("Listening for client account margin");
				var marginListener = _streamingClient.BuildClientAccountMarginListener();
				_listeners.Add(marginListener);
				marginListener.MessageReceived += (s, e) =>
				{
					Console.WriteLine("ClientAccountMargin, equity: {0}", e.Data.NetEquity);
				};

				Console.WriteLine ("Listening for quotes");
				var quotesListener = _streamingClient.BuildQuotesListener();
				_listeners.Add(quotesListener);
				quotesListener.MessageReceived += (s, e) =>
				{
					Console.WriteLine("Quote approved at {0}", e.Data.ApprovalDateTimeUTC);
				};

				Console.WriteLine ("Listening for prices");
				var priceListener = _streamingClient.BuildPricesListener(Convert.ToInt32(ConfigurationManager.AppSettings["MarketId"]));
				_listeners.Add(priceListener);
				priceListener.MessageReceived += (s, message) => { 
					Console.WriteLine("Price at {0:O} is {1}", message.Data.TickDate, message.Data.Price); 
				};

				Console.WriteLine("Listening for 30 seconds - check the number of open connections");
				var gate = new ManualResetEvent(false);
				gate.WaitOne(TimeSpan.FromSeconds(30));
			}
			finally
			{
				Disconnect ();
			}
		}

		static void LogAssembliesBeingUsed ()
		{
			Console.WriteLine ("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
			Console.WriteLine ("Using assembly versions:");
			foreach (var assemblyName in Assembly.GetExecutingAssembly ().GetReferencedAssemblies ().Select (assemblyName => assemblyName.FullName)) {
				Console.WriteLine (assemblyName);
			}
			Console.WriteLine ("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
		}

		public static void Connect() 
		{
			Console.WriteLine ("Connecting to CIAPI");
			_rpcClient = new Client(
				new Uri(ConfigurationManager.AppSettings["Server"]), 
				new Uri(ConfigurationManager.AppSettings["StreamingServer"]), 
				ConfigurationManager.AppSettings["ApiKey"] );
			_rpcClient.LogIn(ConfigurationManager.AppSettings["UserName"], ConfigurationManager.AppSettings["Password"]);
			_streamingClient = _rpcClient.CreateStreamingClient();
		}

		public static void Disconnect()
		{
			Console.WriteLine ("Disconnecting from CIAPI");
			foreach (var listener in _listeners) {
				_streamingClient.TearDownListener (listener);
			}

			_streamingClient.Dispose ();

			_rpcClient.LogOut ();
			_rpcClient.Dispose ();
		}
	}
}
