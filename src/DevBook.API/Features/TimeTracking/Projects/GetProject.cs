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
	public async Task<FieldResult<ProjectDto, NotFoundError>> GetProject(Guid id, DevBookDbContext dbContext, IMapper mapper, CancellationToken cancellationToken)
	{
		var project = await dbContext.Projects.FindAsync([id], cancellationToken: cancellationToken);

		return project is null
			? new NotFoundError { Id = id }
			: mapper.Map<ProjectDto>(project);
	}
}
