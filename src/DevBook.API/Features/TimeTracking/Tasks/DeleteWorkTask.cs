namespace DevBook.API.Features.TimeTracking.Tasks;

public sealed record DeleteWorkTaskInput(Guid Id) : ICommand;

internal class DeleteWorkTaskCommandHandler(DevBookDbContext dbContext) : ICommandHandler<DeleteWorkTaskInput>
{
	public async Task Handle(DeleteWorkTaskInput command, CancellationToken cancellationToken)
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
	public async Task<FieldResult<SuccessResult>> DeleteWorkTask(DeleteWorkTaskInput input, IExecutor executor, CancellationToken cancellationToken)
	{
		await executor.ExecuteCommand(input, cancellationToken);
		return new SuccessResult();
	}
}
