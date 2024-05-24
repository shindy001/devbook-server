using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Xunit.Abstractions;

namespace DevBook.API.IntegrationTests.Helpers;

internal sealed class TimedTestContext<TProgram> : IDisposable where TProgram : class
{
	private readonly Stopwatch _stopWatch;
	private readonly DevBookApiTestFixture<TProgram> _fixture;
	private readonly ITestOutputHelper _outputHelper;

	public TimedTestContext(DevBookApiTestFixture<TProgram> fixture, ITestOutputHelper outputHelper)
	{
		_fixture = fixture;
		_fixture.LoggedMessage += WriteMessage;
		_outputHelper = outputHelper;
		_stopWatch = Stopwatch.StartNew();
	}

	private void WriteMessage(LogLevel logLevel, string category, EventId eventId, string message, Exception? exception)
	{
		var logMessage = $"{_stopWatch.Elapsed.TotalSeconds:N3}s {category} - {logLevel}: {message}";
		if (exception != null)
		{
			logMessage += Environment.NewLine + exception.ToString();
		}
		_outputHelper.WriteLine(logMessage);
	}

	public void Dispose()
	{
		_fixture.LoggedMessage -= WriteMessage;
	}
}
