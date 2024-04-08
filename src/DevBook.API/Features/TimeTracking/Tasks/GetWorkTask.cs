namespace DevBook.API.Features.TimeTracking.Tasks;

internal record GetWorkTaskQuery(Guid Id) : IQuery<OneOf<WorkTaskDto, NotFound>>;

internal class GetWorkTaskQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetWorkTaskQuery, OneOf<WorkTaskDto, NotFound>>
{
	public async Task<OneOf<WorkTaskDto, NotFound>> Handle(GetWorkTaskQuery request, CancellationToken cancellationToken)
	{
		var workTask = await dbContext.Tasks.FindAsync([request.Id], cancellationToken);
		Project? project = workTask?.ProjectId is not null
			? await dbContext.Projects.FindAsync([workTask.ProjectId], cancellationToken)
			: null;

		return workTask is null
			? new NotFound()
			: new WorkTaskDto(
				id: workTask.Id,
				project: project?.ToDto(),
				description: workTask.Description,
				details: workTask.Details,
				date: workTask.Date,
				start: workTask.Start,
				end: workTask.End);
	}
}
