namespace DevBook.API.Features.TimeTracking.Tasks;

internal sealed record ListWorkTasksQuery : IQuery<WorkTaskListResponse>;

internal sealed class ListWorkTasksQueryHandler(DevBookDbContext dbContext) : IQueryHandler<ListWorkTasksQuery, WorkTaskListResponse>
{
	public async Task<WorkTaskListResponse> Handle(ListWorkTasksQuery request, CancellationToken cancellationToken)
	{
		// TODO - implement paging
		var tasks = await dbContext.Tasks
			.ToListAsync(cancellationToken);

		var projectsIds = tasks.Select(x => x.ProjectId);
		var projects = projectsIds.Any()
			? (await dbContext.Projects.Where(proj => projectsIds.Contains(proj.Id)).ToDictionaryAsync(x => x.Id, x => x, cancellationToken))
			: [];

		var taskDtos = tasks.Select(
			task => new WorkTaskDto
			{
				Id = task.Id,
				Project = task.ProjectId is not null && projects.TryGetValue(task.ProjectId.Value, out var project) ? project.ToDto() : null,
				Description = task.Description,
				Details = task.Details,
				Date = task.Date,
				Start = task.Start,
				End = task.End
			});

		var tasksInDays = new Dictionary<DateOnly, IEnumerable<WorkTaskDto>>();
		var dayGroups = taskDtos
			.OrderByDescending(x => x.Date)
			.GroupBy(x => DateOnly.FromDateTime(x.Date.DateTime));

		foreach (var group in dayGroups)
		{
			tasksInDays.Add(group.Key, group.Select(x => x).ToArray());
		}

		return new WorkTaskListResponse
		{
			ActiveWorkTask = taskDtos.FirstOrDefault(x => x.End == null),
			WorkTasksInDay = tasksInDays
		};
	}
}

[QueryType]
internal sealed class WorkTaskListQuery
{
	public async Task<WorkTaskListResponse> GetWorkTaskList(IExecutor executor, CancellationToken cancellationToken)
	{
		return await executor.ExecuteQuery(new ListWorkTasksQuery(), cancellationToken);
	}
}
