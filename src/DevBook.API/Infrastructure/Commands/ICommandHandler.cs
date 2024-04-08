namespace DevBook.API.Infrastructure.Commands;

/// <summary>
/// Defines a handler for a command with void result
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand>
	where TCommand : ICommand;

/// <summary>
/// Defines a handler for a command
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface ICommandHandler<TCommand, TResult> : IRequestHandler<TCommand, TResult>
	where TCommand : ICommand<TResult>;
