namespace DevBook.API.Features.TimeTracking.Tasks;

internal sealed record UpdateWorkTaskCommandDto
{
	public Guid? ProjectId { get; init; }
	public string? Description { get; init; }
	public string? Details { get; init; }

	[Required]
	public required DateTimeOffset Date { get; init; }
	[Required]
	public required TimeOnly Start { get; init; }
	[Required]
	public required TimeOnly End { get; init; }
}

public sealed record UpdateWorkTaskCommand(
	Guid Id,
	Guid? ProjectId,
	string? Description,
	string? Details,
	DateTimeOffset Date,
	TimeOnly Start,
	TimeOnly End)
	: ICommand<OneOf<Success, NotFound>>;

public sealed class UpdateWorkTaskCommandValidator : AbstractValidator<UpdateWorkTaskCommand>
{
	public UpdateWorkTaskCommandValidator()
	{
		RuleFor(x => x.Id).NotEqual(Guid.Empty);
		RuleFor(x => x.End).GreaterThan(x => x.Start);
		When(x => x.ProjectId is not null, () => RuleFor(x => x.ProjectId).NotEqual(Guid.Empty));
	}
}

internal sealed class UpdateUpdateWorkTaskCommandHandler(DevBookDbContext dbContext) : ICommandHandler<UpdateWorkTaskCommand, OneOf<Success, NotFound>>
{
	public async Task<OneOf<Success, NotFound>> Handle(UpdateWorkTaskCommand command, CancellationToken cancellationToken)
	{
		var workTask = await dbContext.Tasks.FindAsync([command.Id], cancellationToken);
		if (workTask is null)
		{
			return new NotFound();
		}
		else
		{
			var update = new Dictionary<string, object?>()
			{
				[nameof(WorkTask.ProjectId)] = command.ProjectId,
				[nameof(WorkTask.Description)] = command.Description,
				[nameof(WorkTask.Details)] = command.Details,
				[nameof(WorkTask.Date)] = command.Date,
				[nameof(WorkTask.Start)] = command.Start,
				[nameof(WorkTask.End)] = command.End,
			};

			dbContext.Tasks.Entry(workTask).CurrentValues.SetValues(update);
			await dbContext.SaveChangesAsync(cancellationToken);
			return new Success();
		}
	}
}

[MutationType]
internal sealed class UpdateWorkTaskMutation
{
	public async Task<FieldResult<WorkTaskDto, NotFoundError>> UpdateWorkTask(UpdateWorkTaskCommand payload, IExecutor executor, IMapper mapper, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(payload, cancellationToken);

		if (result.IsT1)
		{
			return new NotFoundError { Id = payload.Id };
		}

		var item = await executor.ExecuteQuery(new GetWorkTaskQuery(payload.Id), cancellationToken);

		return item.Match<FieldResult<WorkTaskDto, NotFoundError>>(
			workTask => mapper.Map<WorkTaskDto>(workTask),
			notFound => new NotFoundError { Id = payload.Id });
	}
}
