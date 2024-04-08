namespace DevBook.API.Features.TimeTracking.Projects;

public record UpdateProjectCommandDto
{
	[Required]
	public required string Name { get; init; }
	public string? Details { get; init; }
	public int? HourlyRate { get; init; }
	public string? Currency { get; init; }
	public string? HexColor { get; init; }
}

public record UpdateProjectCommand(
	Guid Id,
	string Name,
	string? Details,
	int? HourlyRate,
	string? Currency,
	string? HexColor)
	: ICommand<OneOf<Success, NotFound>>;

public sealed class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
	public UpdateProjectCommandValidator()
	{
		RuleFor(x => x.Id).NotEqual(Guid.Empty);
		RuleFor(x => x.Name).NotEmpty();
	}
}

internal sealed class UpdateProjectCommandHandler(DevBookDbContext dbContext) : ICommandHandler<UpdateProjectCommand, OneOf<Success, NotFound>>
{
	public async Task<OneOf<Success, NotFound>> Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
	{
		var project = await dbContext.Projects.FindAsync([command.Id], cancellationToken);
		if (project is null)
		{
			return new NotFound();
		}
		else
		{
			var update = new Dictionary<string, object?>()
			{
				[nameof(Project.Name)] = command.Name,
				[nameof(Project.Details)] = command.Details,
				[nameof(Project.HourlyRate)] = command.HourlyRate,
				[nameof(Project.Currency)] = command.Currency,
				[nameof(Project.HexColor)] = command.HexColor,
			};

			dbContext.Projects.Entry(project).CurrentValues.SetValues(update);
			await dbContext.SaveChangesAsync(cancellationToken);
			return new Success();
		}
	}
}
