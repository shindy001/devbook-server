namespace DevBook.API.Infrastructure.Queries;

/// <summary>
/// Represent Query action with a result
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IQuery<TResult> : IRequest<TResult>;
