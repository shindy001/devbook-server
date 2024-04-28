namespace DevBook.API.Features.TimeTracking.Projects;

internal record GetProjectQuery(Guid Id) : IQuery<OneOf<Project, NotFound>>;

internal class GetProjectQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetProjectQuery, OneOf<Project, NotFound>>
{
	public async Task<OneOf<Project, NotFound>> Handle(GetProjectQuery request, CancellationToken cancellationToken)
	{
		var project = await dbContext.Projects.FindAsync([request.Id], cancellationToken);

		return project is null
			? new NotFound()
			: project;
	}
}

[QueryType]
internal sealed class ProjectQuery
{
	public async Task<FieldResult<ProjectDto, NotFoundError>> GetProject(GetProjectQuery input, IExecutor executor, IMapper mapper, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(input, cancellationToken);

		return result.Match<FieldResult<ProjectDto, NotFoundError>>(
			project => mapper.Map<ProjectDto>(project),
			notFound => new NotFoundError { Id = input.Id });
	}
}
