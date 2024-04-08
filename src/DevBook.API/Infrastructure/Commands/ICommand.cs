namespace DevBook.API.Infrastructure.Commands;

/// <summary>
/// Represent Command action that does not return any value
/// </summary>
public interface ICommand : IRequest;

/// <summary>
/// Represents Command action that returns a result
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface ICommand<TResult> : IRequest<TResult>;
