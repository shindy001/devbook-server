namespace DevBook.API.Features.TimeTracking.Projects;

public sealed record GetProjectInput(Guid Id) : IQuery<OneOf<Project, NotFound>>;

internal class GetProjectQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetProjectInput, OneOf<Project, NotFound>>
{
	public async Task<OneOf<Project, NotFound>> Handle(GetProjectInput request, CancellationToken cancellationToken)
	{
		var project = await dbContext.Projects.FindAsync([request.Id], cancellationToken);

		return project is null
			? new NotFound()
			: project;
	}
}

[QueryType]
internal sealed class GetProjectQuery
{
	public async Task<FieldResult<ProjectDto, NotFoundError>> GetProject(GetProjectInput input, IExecutor executor, IMapper mapper, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(input, cancellationToken);

		return result.Match<FieldResult<ProjectDto, NotFoundError>>(
			project => mapper.Map<ProjectDto>(project),
			notFound => new NotFoundError { Id = input.Id });
	}
}
