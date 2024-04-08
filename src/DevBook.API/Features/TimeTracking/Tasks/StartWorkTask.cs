namespace DevBook.API.Features.TimeTracking.Tasks;

internal sealed record StartWorkTaskCommand : ICommand<Guid>
{
	public string? Description { get; init; }
	public required DateTimeOffset Date { get; init; }
	public required TimeOnly Start { get; init; }
}

internal sealed class StartTaskCommandHandler(DevBookDbContext dbContext) : ICommandHandler<StartWorkTaskCommand, Guid>
{
	public async Task<Guid> Handle(StartWorkTaskCommand request, CancellationToken cancellationToken)
	{
		if (await dbContext.Tasks.AnyAsync(x => x.End == null, cancellationToken: cancellationToken))
		{
			throw new DevBookValidationException(nameof(request.Description), $"Cannot start task, there is already a running task.");
		}

		var newItem = new WorkTask(
			ProjectId: null,
			Description: request.Description,
			Details: null,
			Date: request.Date.UtcDateTime,
			Start: request.Start,
			End: null);

		await dbContext.Tasks.AddAsync(newItem, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
		return newItem.Id;
	}
}
