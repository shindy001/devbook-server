using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

namespace DevBook.API;

public static class ServiceDefaults
{
	/// <summary>
	/// Adds healthcheck, http resiliency and default logging
	/// </summary>
	/// <param name="builder"></param>
	/// <returns></returns>
	public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
	{
		builder.Services.AddHealthChecks()
			// Add a default liveness check to ensure app is responsive
			.AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

		builder.Services.ConfigureHttpClientDefaults(http =>
		{
			// Turn on resilience by default
			http.AddStandardResilienceHandler();
		});

		builder.Services.AddLogging(cfg => cfg.AddDevBookLogging());

		return builder;
	}

	/// <summary>
	/// Adds console and file logging, saves logs to [AppDomain.CurrentDomain.BaseDirectory]/logs/[log Name]
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="logFileName"></param>
	private static ILoggingBuilder AddDevBookLogging(this ILoggingBuilder builder, string logFileName = "devbook_log_.txt")
	{
		var appDir = AppDomain.CurrentDomain.BaseDirectory;
		var outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {CorrelationId} {Level:u3}] {Message:lj}{NewLine}{Exception}";

		Log.Logger = new LoggerConfiguration()
			.WriteTo.Console(outputTemplate: outputTemplate)
			.WriteTo.File(
				path: System.IO.Path.Combine(appDir, "logs", logFileName),
				rollingInterval: RollingInterval.Day,
				outputTemplate: outputTemplate)
			.CreateLogger();

		builder.AddSerilog(Log.Logger);

		return builder;
	}

	/// <summary>
	/// Maps Alive and Health endpoints
	/// </summary>
	/// <param name="app"></param>
	/// <returns></returns>
	public static WebApplication MapDefaultEndpoints(this WebApplication app)
	{
		// All health checks must pass for app to be considered ready to accept traffic after starting
		app.MapHealthChecks("/health");

		// Only health checks tagged with the "live" tag must pass for app to be considered alive
		app.MapHealthChecks("/alive", new HealthCheckOptions
		{
			Predicate = r => r.Tags.Contains("live")
		});

		return app;
	}
}
