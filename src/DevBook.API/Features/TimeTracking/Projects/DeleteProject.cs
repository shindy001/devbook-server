namespace DevBook.API.Features.TimeTracking.Projects;

public sealed record DeleteProjectInput(Guid Id) : ICommand;

internal class DeleteProjectCommandHandler(DevBookDbContext dbContext) : ICommandHandler<DeleteProjectInput>
{
	public async Task Handle(DeleteProjectInput command, CancellationToken cancellationToken)
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
	public async Task<FieldResult<SuccessResult>> DeleteProject(DeleteProjectInput input, IExecutor executor, CancellationToken cancellationToken)
	{
		await executor.ExecuteCommand(input, cancellationToken);
		return new SuccessResult();
	}
}
