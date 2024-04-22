namespace DevBook.API.Features.TimeTracking.Projects;

public record PatchProjectCommandDto(
	string? Name,
	string? Details,
	int? HourlyRate,
	string? Currency,
	string? HexColor);

public record PatchProjectCommand(
	Guid Id,
	string? Name,
	string? Details,
	int? HourlyRate,
	string? Currency,
	string? HexColor)
	: ICommand<OneOf<Success, NotFound>>;

public sealed class PatchProjectCommandValidator : AbstractValidator<PatchProjectCommand>
{
	public PatchProjectCommandValidator()
	{
		RuleFor(x => x.Id).NotEqual(Guid.Empty);
	}
}

internal sealed class PatchProjectCommandHandler(DevBookDbContext dbContext) : ICommandHandler<PatchProjectCommand, OneOf<Success, NotFound>>
{
	public async Task<OneOf<Success, NotFound>> Handle(PatchProjectCommand command, CancellationToken cancellationToken)
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
	public async Task<FieldResult<ProjectDto, NotFoundError>> PatchProject(PatchProjectCommand payload, IExecutor executor, IMapper mapper, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(
			new PatchProjectCommand(
				Id: payload.Id,
				Name: payload.Name,
				Details: payload.Details,
				HourlyRate: payload.HourlyRate,
				Currency: payload.Currency,
				HexColor: payload.HexColor),
		cancellationToken);

		if (result.IsT1)
		{
			return new NotFoundError { Id = payload.Id };
		}

		var item = await executor.ExecuteQuery(new GetProjectQuery(payload.Id), cancellationToken);

		return item.Match<FieldResult<ProjectDto, NotFoundError>>(
			project => mapper.Map<ProjectDto>(project),
			notFound => new NotFoundError { Id = payload.Id });
	}
}
