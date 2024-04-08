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
