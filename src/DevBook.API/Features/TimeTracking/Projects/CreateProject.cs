namespace DevBook.API.Features.TimeTracking.Projects;

public sealed record CreateProjectCommand : ICommand<Project>
{
	[Required]
	public required string Name { get; init; }
	public string? Details { get; init; }
	public int? HourlyRate { get; init; }
	public string? Currency { get; init; }
	public string? HexColor { get; init; }
}

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
	public CreateProjectCommandValidator()
	{
		RuleFor(x => x.Name).NotEmpty();
	}
}

internal sealed class CreateProjectCommandHandler(DevBookDbContext dbContext) : ICommandHandler<CreateProjectCommand, Project>
{
	public async Task<Project> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
	{
		var newItem = new Project { Name = request.Name, Details = request.Details, HourlyRate = request.HourlyRate, Currency = request.Currency, HexColor = request.HexColor };
		await dbContext.Projects.AddAsync(newItem, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
		return newItem;
	}
}

[MutationType]
internal sealed class CreateProjectMutation
{
	public async Task<ProjectDto> CreateProject(CreateProjectCommand input, IExecutor executor, IMapper mapper, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(input, cancellationToken);
		return mapper.Map<ProjectDto>(result);
	}
}
