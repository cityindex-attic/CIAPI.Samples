using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Common.Logging;
using Common.Logging.Simple;

namespace CiapiNewsTest
{
	class MyLogger : AbstractSimpleLogger
	{
		public MyLogger(string logName, LogLevel logLevel, bool showlevel, bool showDateTime,
				bool showLogName, string dateTimeFormat)
			: base(logName, logLevel, showlevel, showDateTime, showLogName, dateTimeFormat)
		{
		}

		protected override void WriteInternal(LogLevel level, object message, Exception exception)
		{
			OnMessage(level, message, exception);
		}

		public Action<LogLevel, object, Exception> OnMessage;
	}

	class MyLoggerFactoryAdapter : AbstractSimpleLoggerFactoryAdapter
	{
		public MyLoggerFactoryAdapter(NameValueCollection properties)
			: base(properties)
		{
		}

		protected override ILog CreateLogger(string name, LogLevel level, bool showLevel,
			bool showDateTime, bool showLogName, string dateTimeFormat)
		{
			var res = new MyLogger(name, level, showLevel, showDateTime, showLogName, dateTimeFormat);
			res.OnMessage = OnMessage;
			return res;
		}

		public Action<LogLevel, object, Exception> OnMessage;
	}
}
