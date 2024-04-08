namespace DevBook.API.Infrastructure.Commands;

internal sealed class UnitOfWorkCommandPipelineBehavior<TCommand, TResult>
	: ICommandPipelineBehavior<TCommand, TResult>
	where TCommand : ICommand<TResult>
{
	private readonly IUnitOfWork _unitOfWork;

	public UnitOfWorkCommandPipelineBehavior(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

	public async Task<TResult> Handle(TCommand request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
	{
		var response = await next();
		await _unitOfWork.CommitAsync(cancellationToken);
		return response;
	}
}
