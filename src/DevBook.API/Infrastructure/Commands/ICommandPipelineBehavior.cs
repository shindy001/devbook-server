using MediatR;

namespace DevBook.API.Infrastructure.Commands;

/// <summary>
/// Pipeline behavior to surround the inner command handler.
/// Implementations add additional behavior and await the next delegate.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface ICommandPipelineBehavior<in TCommand, TResult>
	: IPipelineBehavior<TCommand, TResult>
	where TCommand : ICommand<TResult>;
