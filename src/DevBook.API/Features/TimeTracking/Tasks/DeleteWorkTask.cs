namespace DevBook.API.Features.TimeTracking.Tasks;

public sealed record DeleteWorkTaskCommand(Guid Id) : ICommand;

internal class DeleteWorkTaskCommandHandler(DevBookDbContext dbContext) : ICommandHandler<DeleteWorkTaskCommand>
{
	public async Task Handle(DeleteWorkTaskCommand command, CancellationToken cancellationToken)
	{
		var workTask = await dbContext.Tasks.FindAsync([command.Id], cancellationToken);

		if (workTask is not null)
		{
			dbContext.Tasks.Remove(workTask);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}

[MutationType]
internal sealed class DeleteWorkTaskMutation
{
	public async Task<FieldResult<SuccessResult>> DeleteWorkTask(DeleteWorkTaskCommand payload, IExecutor executor, CancellationToken cancellationToken)
	{
		await executor.ExecuteCommand(payload, cancellationToken);
		return new SuccessResult();
	}
}
