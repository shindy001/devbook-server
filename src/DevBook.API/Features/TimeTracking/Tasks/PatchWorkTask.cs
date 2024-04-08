namespace DevBook.API.Features.TimeTracking.Tasks;

internal sealed record PatchWorkTaskCommandDto
{
	public Guid? ProjectId { get; init; }
	public string? Description { get; init; }
	public string? Details { get; init; }
	public DateTimeOffset? Date { get; init; }
	public TimeOnly? Start { get; init; }
	public TimeOnly? End { get; init; }
}

public record PatchWorkTaskCommand(
	Guid Id,
	Guid? ProjectId,
	string? Description,
	string? Details,
	DateTimeOffset? Date,
	TimeOnly? Start,
	TimeOnly? End)
	: ICommand<OneOf<Success, NotFound>>;

public sealed class PatchWorkTaskCommandValidator : AbstractValidator<PatchWorkTaskCommand>
{
	public PatchWorkTaskCommandValidator()
	{
		RuleFor(x => x.Id).NotEqual(Guid.Empty);
		When(x => x.ProjectId is not null, () => RuleFor(x => x.ProjectId).NotEqual(Guid.Empty));
	}
}

internal sealed class PatchProjectCommandHandler(DevBookDbContext dbContext) : ICommandHandler<PatchWorkTaskCommand, OneOf<Success, NotFound>>
{
	public async Task<OneOf<Success, NotFound>> Handle(PatchWorkTaskCommand command, CancellationToken cancellationToken)
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
				[nameof(WorkTask.ProjectId)] = command.ProjectId ?? workTask.ProjectId,
				[nameof(WorkTask.Description)] = command.Description ?? workTask.Description,
				[nameof(WorkTask.Details)] = command.Details ?? workTask.Details,
				[nameof(WorkTask.Date)] = command.Date ?? workTask.Date,
				[nameof(WorkTask.Start)] = command.Start ?? workTask.Start,
				[nameof(WorkTask.End)] = command.End ?? workTask.End,
			};

			dbContext.Tasks.Entry(workTask).CurrentValues.SetValues(update);
			await dbContext.SaveChangesAsync(cancellationToken);
			return new Success();
		}
	}
}
