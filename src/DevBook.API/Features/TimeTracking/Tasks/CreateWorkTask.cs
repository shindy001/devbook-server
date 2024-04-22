namespace DevBook.API.Features.TimeTracking.Tasks;

public sealed record CreateWorkTaskCommand : ICommand<WorkTask>
{
	public Guid? ProjectId { get; init; }
	public string? Description { get; init; }
	public string? Details { get; init; }
	public required DateTimeOffset Date { get; init; }
	public required TimeOnly Start { get; init; }
	public required TimeOnly End { get; init; }
}

public sealed class CreateWorkTaskCommandValidator : AbstractValidator<CreateWorkTaskCommand>
{
	public CreateWorkTaskCommandValidator()
	{
		When(x => x.ProjectId is not null, () => RuleFor(x => x.ProjectId).NotEqual(Guid.Empty));
		RuleFor(x => x.End).GreaterThan(x => x.Start);
	}
}

internal sealed class CreateTaskCommandHandler(DevBookDbContext dbContext) : ICommandHandler<CreateWorkTaskCommand, WorkTask>
{
	public async Task<WorkTask> Handle(CreateWorkTaskCommand request, CancellationToken cancellationToken)
	{
		if (request.ProjectId is not null && !(await dbContext.Projects.AnyAsync(x => x.Id.Equals(request.ProjectId), cancellationToken: cancellationToken)))
		{
			throw new DevBookValidationException(nameof(request.ProjectId), $"Project with id '{request.ProjectId}' not found.");
		}

		var newItem = new WorkTask(
			ProjectId: request.ProjectId,
			Description: request.Description,
			Details: request.Details,
			Date: request.Date.UtcDateTime,
			Start: request.Start,
			End: request.End);

		await dbContext.Tasks.AddAsync(newItem, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
		return newItem;
	}
}

[MutationType]
internal sealed class CreateWorkTaskMutation
{
	public async Task<WorkTaskDto> CreateWorkTask(CreateWorkTaskCommand payload, IExecutor executor, IMapper mapper, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(payload, cancellationToken);
		return mapper.Map<WorkTaskDto>(result);
	}
}
