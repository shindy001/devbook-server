namespace DevBook.API.Features.TimeTracking.Tasks;

internal record DeleteWorkTaskCommand(Guid Id) : ICommand;

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
