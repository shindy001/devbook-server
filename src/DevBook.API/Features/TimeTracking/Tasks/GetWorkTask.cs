using DevBook.API.Features.TimeTracking.Shared;

namespace DevBook.API.Features.TimeTracking.Tasks;

public sealed record WorkTaskInput(Guid Id) : IQuery<OneOf<WorkTaskDto, NotFound>>;

internal class GetWorkTaskQueryHandler(DevBookDbContext dbContext) : IQueryHandler<WorkTaskInput, OneOf<WorkTaskDto, NotFound>>
{
	public async Task<OneOf<WorkTaskDto, NotFound>> Handle(WorkTaskInput request, CancellationToken cancellationToken)
	{
		var workTask = await dbContext.Tasks.FindAsync([request.Id], cancellationToken);
		Project? project = workTask?.ProjectId is not null
			? await dbContext.Projects.FindAsync([workTask.ProjectId], cancellationToken)
			: null;

		return workTask is null
			? new NotFound()
			: new WorkTaskDto
				{
					Id = workTask.Id,
					Project = project?.ToDto(),
					Description = workTask.Description,
					Details = workTask.Details,
					Date = workTask.Date,
					Start = workTask.Start,
					End = workTask.End
				};
	}
}

[QueryType]
internal sealed class WorkTaskQuery
{
	public async Task<FieldResult<WorkTaskDto, NotFoundError>> GetWorkTask(WorkTaskInput input, IExecutor executor, IMapper mapper, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(input, cancellationToken);

		return result.Match<FieldResult<WorkTaskDto, NotFoundError>>(
			workTask => mapper.Map<WorkTaskDto>(workTask),
			notFound => new NotFoundError { Id = input.Id });
	}
}
