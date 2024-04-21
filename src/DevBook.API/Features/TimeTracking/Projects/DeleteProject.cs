namespace DevBook.API.Features.TimeTracking.Projects;

internal record DeleteProjectCommand(Guid Id) : ICommand;

internal class DeleteProjectCommandHandler(DevBookDbContext dbContext) : ICommandHandler<DeleteProjectCommand>
{
	public async Task Handle(DeleteProjectCommand command, CancellationToken cancellationToken)
	{
		var project = await dbContext.Projects.FindAsync([command.Id], cancellationToken);

		if (project is not null)
		{
			dbContext.Projects.Remove(project);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}

[MutationType]
internal sealed class DeleteProjectMutation
{
	public async Task<FieldResult<SuccessResult>> DeleteProject(Guid id, IExecutor executor, CancellationToken cancellationToken)
	{
		await executor.ExecuteCommand(new DeleteProjectCommand(id), cancellationToken);
		return new SuccessResult();
	}
}
