using System.Reflection;

namespace DevBook.API.Infrastructure;

/// <summary>
/// Represents an entity which handles execution of queries and commands
/// </summary>
public interface IExecutor
{
	Task<TResult> ExecuteQuery<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

	Task<TResult> ExecuteCommand<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);

	Task ExecuteCommand(ICommand command, CancellationToken cancellationToken = default);
}

public sealed class Executor(ISender mediator) : IExecutor
{
	public async Task<TResult> ExecuteQuery<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
		=> await mediator.Send(query, cancellationToken);

	public async Task<TResult> ExecuteCommand<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
		=> await mediator.Send(command, cancellationToken);

	public async Task ExecuteCommand(ICommand command, CancellationToken cancellationToken = default)
		=> await mediator.Send(command, cancellationToken);
}

public static class ExecutorExtensions
{
	/// <summary>
	/// Registers <see cref="IExecutor"/> scoped service and discovered command/query handlers
	/// </summary>
	/// <param name="services"></param>
	/// <param name="commandAndQueriesAssemblies"></param>
	/// <returns></returns>
	public static IServiceCollection AddCommandsAndQueriesExecutor(this IServiceCollection services, params Assembly[] commandAndQueriesAssemblies)
	{
		services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(commandAndQueriesAssemblies));
		services.AddScoped<IExecutor, Executor>();

		return services;
	}

	/// <summary>
	/// Registers passed behavior as generic scoped pipeline behavior <see cref="IPipelineBehavior{TRequest, TResponse}"/>
	/// </summary>
	/// <param name="services"></param>
	/// <param name="pipelineBehavior"></param>
	/// <returns></returns>
	public static IServiceCollection AddPipelineBehavior(this IServiceCollection services, Type pipelineBehavior)
	{
		services.AddScoped(typeof(IPipelineBehavior<,>), pipelineBehavior);

		return services;
	}
}
