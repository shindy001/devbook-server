using DevBook.API.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DevBook.API.IntegrationTests.Helpers;

public delegate void LogMessage(LogLevel logLevel, string categoryName, EventId eventId, string message, Exception? exception);

public sealed class DevBookApiTestFixture<TProgram> : IDisposable where TProgram : class
{
	private WebApplicationFactory<TProgram>? _app;
	private TestServer? _server;
	private HttpClient? _client;
	private List<Action<IWebHostBuilder>> _configureWebHostActions = [];

	private readonly string TestDbName = $"integrationTests-{Guid.NewGuid()}.db";

	public event LogMessage? LoggedMessage;
	public LoggerFactory LoggerFactory { get; }

	public DevBookApiTestFixture()
	{
		LoggerFactory = new LoggerFactory();
		LoggerFactory.AddProvider(new ForwardingLoggerProvider((logLevel, category, eventId, message, exception)
			=> LoggedMessage?.Invoke(logLevel, category, eventId, message, exception)));
	}

	/// <summary>
	/// Replaces specified service in DI container, should be called before <see cref="CreateClient"/>.
	/// </summary>
	/// <typeparam name="TService"></typeparam>
	/// <param name="instance"></param>
	/// <exception cref="InvalidOperationException"></exception>
	public void ReplaceService<TService>(object instance)
	{
		_configureWebHostActions.Add((builder) =>
		{
			builder.ConfigureServices(
				services =>
				{
					var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TService));
					if (descriptor != null)
					{
						services.Replace(new ServiceDescriptor(typeof(TService), provider => instance!, descriptor.Lifetime));
					}
					else
					{
						throw new InvalidOperationException($"Service of type '{typeof(TService).GetType().Name}' not found hence cannot be replaced. Make sure you are trying to replace correct service.");
					}
				});
		});
	}

	public IDisposable GetTestContext(ITestOutputHelper outputHelper)
	{
		return new TimedTestContext<TProgram>(this, outputHelper);
	}

	public HttpClient CreateClient()
	{
		if (_client is null)
		{
			_app = new WebApplicationFactory<TProgram>()
				.WithWebHostBuilder(builder =>
				{
					builder.ConfigureServices(services =>
					{
						services.AddSingleton<ILoggerFactory>(LoggerFactory);

						ReplaceDbWithTestDb(services);
					});

					builder.UseTestServer();
					foreach (var action in _configureWebHostActions)
					{
						action?.Invoke(builder);
					}
				});

			_client = _app.CreateClient();
			_server = (TestServer)_app.Services.GetRequiredService<IServer>();
		}

		return _client;
	}

	private void ReplaceDbWithTestDb(IServiceCollection services)
	{
		var dbContextDescriptor = services.SingleOrDefault(
			d => d.ServiceType == typeof(DbContextOptions<DevBookDbContext>));

		if (dbContextDescriptor != null)
		{
			services.Remove(dbContextDescriptor);
		}

		services.AddDbContextPool<DevBookDbContext>(
			o => o.UseSqlite($"Data Source={TestDbName};Pooling=false", // Disabled pooling so the db file is unlocked after dbcontext dispose and can be deleted
			b => b.MigrationsAssembly(typeof(TProgram).Assembly.GetName().Name)));
	}

	public void Dispose()
	{
		_app?.Dispose();
		_client?.Dispose();
		_server?.Dispose();

		// Clean test db file after run - does not work when pooled dbcontext is used(default behavior), pooling locks db file
		File.Delete(TestDbName);
	}
}
