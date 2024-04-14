namespace DevBook.API.Features.TimeTracking.Projects;

internal sealed record GetProjectsQuery : IQuery<IEnumerable<Project>>;

internal sealed class GetProjectsQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetProjectsQuery, IEnumerable<Project>>
{
	public async Task<IEnumerable<Project>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
	{
		// TODO - implement paging
		return await dbContext.Projects.ToListAsync(cancellationToken);
	}
}

public sealed class ProjectsQuery
{
	public async Task<IEnumerable<ProjectDto>> GetProjects([Service] DevBookDbContext dbContext, CancellationToken cancellationToken)
	{
		var projects = await dbContext.Projects.ToListAsync(cancellationToken);
		return projects.Select(x => x.ToDto());
	}
}
