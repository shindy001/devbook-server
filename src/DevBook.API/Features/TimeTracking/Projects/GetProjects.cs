using DevBook.API.Features.TimeTracking.Shared;
using HotChocolate.Resolvers;

namespace DevBook.API.Features.TimeTracking.Projects;

internal sealed record GetProjectsQuery : IQuery<IQueryable<Project>>;

internal sealed class GetProjectsQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetProjectsQuery, IQueryable<Project>>
{
	public Task<IQueryable<Project>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
	{
		// TODO - implement paging
		return Task.FromResult(dbContext.Projects.AsQueryable());
	}
}

[QueryType]
internal sealed class ProjectsQuery
{
	[UseProjection]
	public async Task<IQueryable<ProjectDto>> GetProjects(IExecutor executor, IResolverContext resolverContext, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(new GetProjectsQuery(), cancellationToken);
		return result.ProjectTo<Project, ProjectDto>(resolverContext);
	}
}
