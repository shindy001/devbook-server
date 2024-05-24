using Microsoft.Extensions.Logging;

namespace DevBook.API.IntegrationTests.Helpers;

internal sealed class ForwardingLoggerProvider : ILoggerProvider
{
	private readonly LogMessage _logMessage;

	public ForwardingLoggerProvider(LogMessage logMessage)
	{
		_logMessage = logMessage;
	}

	public ILogger CreateLogger(string categoryName)
	{
		return new ForwardingLogger(categoryName, _logMessage);
	}

	public void Dispose()
	{
	}

	internal sealed class ForwardingLogger : ILogger
	{
		private readonly string _categoryName;
		private readonly LogMessage _logMessage;

		public ForwardingLogger(string categoryName, LogMessage logMessage)
		{
			_categoryName = categoryName;
			_logMessage = logMessage;
		}

		public IDisposable? BeginScope<TState>(TState state) where TState : notnull
		{
			return null;
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
			_logMessage(logLevel, _categoryName, eventId, formatter(state, exception), exception);
		}
	}
}
