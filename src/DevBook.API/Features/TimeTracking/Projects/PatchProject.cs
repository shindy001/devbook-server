namespace DevBook.API.Features.TimeTracking.Projects;

public record PatchProjectCommandDto(
	string? Name,
	string? Details,
	int? HourlyRate,
	string? Currency,
	string? HexColor);

public record PatchProjectInput(
	Guid Id,
	string? Name,
	string? Details,
	int? HourlyRate,
	string? Currency,
	string? HexColor)
	: ICommand<OneOf<Success, NotFound>>;

public sealed class PatchProjectCommandValidator : AbstractValidator<PatchProjectInput>
{
	public PatchProjectCommandValidator()
	{
		RuleFor(x => x.Id).NotEqual(Guid.Empty);
	}
}

internal sealed class PatchProjectCommandHandler(DevBookDbContext dbContext) : ICommandHandler<PatchProjectInput, OneOf<Success, NotFound>>
{
	public async Task<OneOf<Success, NotFound>> Handle(PatchProjectInput command, CancellationToken cancellationToken)
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
				[nameof(Project.Name)] = command.Name ?? project.Name,
				[nameof(Project.Details)] = command.Details ?? project.Details,
				[nameof(Project.HourlyRate)] = command.HourlyRate ?? project.HourlyRate,
				[nameof(Project.Currency)] = command.Currency ?? project.Currency,
				[nameof(Project.HexColor)] = command.HexColor ?? project.HexColor,
			};

			dbContext.Projects.Entry(project).CurrentValues.SetValues(update);
			await dbContext.SaveChangesAsync(cancellationToken);
			return new Success();
		}
	}
}

[MutationType]
internal sealed class PatchProjectMutation
{
	public async Task<FieldResult<ProjectDto, NotFoundError>> PatchProject(PatchProjectInput input, IExecutor executor, IMapper mapper, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(input, cancellationToken);

		if (result.IsT1)
		{
			return new NotFoundError { Id = input.Id };
		}

		var item = await executor.ExecuteQuery(new GetProjectInput(input.Id), cancellationToken);

		return item.Match<FieldResult<ProjectDto, NotFoundError>>(
			project => mapper.Map<ProjectDto>(project),
			notFound => new NotFoundError { Id = input.Id });
	}
}
