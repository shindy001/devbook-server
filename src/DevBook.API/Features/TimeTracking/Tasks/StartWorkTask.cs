namespace DevBook.API.Features.TimeTracking.Tasks;

public sealed record StartWorkTaskCommand : ICommand<OneOf<WorkTask, DevBookValidationException>>
{
	public string? Description { get; init; }
	public required DateTimeOffset Date { get; init; }
	public required TimeOnly Start { get; init; }
}

internal sealed class StartTaskCommandHandler(DevBookDbContext dbContext) : ICommandHandler<StartWorkTaskCommand, OneOf<WorkTask, DevBookValidationException>>
{
	public async Task<OneOf<WorkTask, DevBookValidationException>> Handle(StartWorkTaskCommand request, CancellationToken cancellationToken)
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
		return newItem;
	}
}

[MutationType]
internal sealed class StartWorkTaskMutation
{
	public async Task<WorkTaskDto> StartWorkTask(StartWorkTaskCommand payload, IExecutor executor, IMapper mapper, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(payload, cancellationToken);

		return result.Match(
			mapper.Map<WorkTaskDto>,
			validationException => throw new GraphQLException(validationException.Message));
	}
}
