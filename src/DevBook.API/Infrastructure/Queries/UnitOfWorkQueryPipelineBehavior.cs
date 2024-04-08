namespace DevBook.API.Infrastructure.Queries;

public class UnitOfWorkQueryPipelineBehavior<TQuery, TResult>
	: IQueryPipelineBehavior<TQuery, TResult>
	where TQuery : IQuery<TResult>
{
	private readonly IUnitOfWork _unitOfWork;

	public UnitOfWorkQueryPipelineBehavior(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

	public async Task<TResult> Handle(TQuery request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
	{
		_unitOfWork.AsNoTrackingQuery();
		return await next();
	}
}
