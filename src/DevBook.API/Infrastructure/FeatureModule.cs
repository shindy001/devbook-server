using System.Reflection;

namespace DevBook.API.Infrastructure;

/// <summary>
/// Marks feature module
/// Modules are automatically discovered and registered by <see cref="FeatureModuleManager"/> methods.
/// </summary>
public interface IFeatureModule
{
	/// <summary>
	/// Register dependencies required by the module
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	IServiceCollection RegisterModule(IServiceCollection services);

	/// <summary>
	/// Runs initialization required by the module in service scope
	/// </summary>
	/// <param name="serviceScope"></param>
	/// <returns></returns>
	Task InitializeModule(AsyncServiceScope serviceScope);

	/// <summary>
	/// Map endpoints required by the module
	/// </summary>
	/// <param name="endpointsBuilder"></param>
	/// <returns></returns>
	IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpointsBuilder);
}

public sealed class FeatureModuleManager
{
	private readonly List<IFeatureModule> _registeredModules = [];

	/// <summary>
	/// Registers modules that implement <see cref="IFeatureModule"/> contract
	/// </summary>
	/// <param name="services"></param>
	/// <param name="commandAndQueriesAssemblies"></param>
	/// <returns></returns>
	public IServiceCollection RegisterFeatureModules(IServiceCollection services, params Assembly[] commandAndQueriesAssemblies)
	{
		foreach (var assembly in commandAndQueriesAssemblies)
		{
			RegisterModules(services, assembly);
		}

		return services;
	}

	/// <summary>
	/// Maps <see cref="IFeatureModule"/> enpoints
	/// </summary>
	/// <param name="app"></param>
	/// <returns></returns>
	public WebApplication MapFeatureModulesEndpoints(WebApplication app)
	{
		foreach (var module in this._registeredModules)
		{
			module.MapEndpoints(app);
		}

		return app;
	}

	/// <summary>
	/// Runs module initialization <see cref="IFeatureModule"/>
	/// </summary>
	/// <param name="appBuilder"></param>
	/// <returns></returns>
	public async Task InitializeModules(IApplicationBuilder appBuilder)
	{
		foreach (var module in this._registeredModules)
		{
			await using var scope = appBuilder.ApplicationServices.CreateAsyncScope();
			await module.InitializeModule(scope);
		}
	}

	private void RegisterModules(IServiceCollection services, Assembly assembly)
	{
		var modules = DiscoverModules(assembly);
		foreach (var module in modules)
		{
			module.RegisterModule(services);
			this._registeredModules.Add(module);
		}
	}

	private static IEnumerable<IFeatureModule> DiscoverModules(Assembly assembly)
	{
		return assembly
			.GetTypes()
			.Where(t => t.IsClass && t.IsAssignableTo(typeof(IFeatureModule)))
			.Select(Activator.CreateInstance)
			.Cast<IFeatureModule>();
	}
}
