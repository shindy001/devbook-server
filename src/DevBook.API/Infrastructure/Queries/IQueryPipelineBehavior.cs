namespace DevBook.API.Infrastructure.Queries;

/// <summary>
/// Pipeline behavior to surround the inner query handler.
/// Implementations add additional behavior and await the next delegate.
/// </summary>
/// <typeparam name="TQuery"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface IQueryPipelineBehavior<TQuery, TResult>
	: IPipelineBehavior<TQuery, TResult>
	where TQuery : IQuery<TResult>;
