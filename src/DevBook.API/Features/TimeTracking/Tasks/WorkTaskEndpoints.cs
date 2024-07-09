namespace DevBook.API.Features.TimeTracking.Tasks;

internal static class WorkTaskEndpoints
{
	private const string OperationIdPrefix = "WorkTasks.";

	public static RouteGroupBuilder MapWorkTaskEndpoints(this RouteGroupBuilder groupBuilder)
	{
		groupBuilder.MapGet("/", ListWorkTasks)
			.WithName($"{OperationIdPrefix}List")
			.Produces<WorkTaskListResponse>();

		groupBuilder.MapPost("/", CreateWorkTask)
			.WithName($"{OperationIdPrefix}Create")
			.Produces(StatusCodes.Status201Created);

		groupBuilder.MapPost("/start", StartWorkTask)
			.WithName($"{OperationIdPrefix}Start")
			.Produces(StatusCodes.Status201Created);

		groupBuilder.MapGet("/{id:guid}", GetWorkTaskById)
			.WithName($"{OperationIdPrefix}{ApiConstants.GetByIdRoute}")
			.Produces<WorkTaskDto>()
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapPut("/{id:guid}", UpdateWorkTask)
			.WithName($"{OperationIdPrefix}Update")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapPatch("/{id:guid}", PatchWorkTask)
			.WithName($"{OperationIdPrefix}Patch")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapDelete("/{id:guid}", DeleteWorkTask)
			.WithName($"{OperationIdPrefix}Delete")
			.Produces(StatusCodes.Status204NoContent);

		return groupBuilder;
	}

	private static async Task<IResult> ListWorkTasks(IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(new ListWorkTasksQuery(), cancellationToken);
		return TypedResults.Ok(result);
	}

	private static async Task<IResult> CreateWorkTask(CreateWorkTaskInput command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(command, cancellationToken);
		return TypedResults.CreatedAtRoute($"{OperationIdPrefix}{ApiConstants.GetByIdRoute}", new { id = result.Id });
	}

	private static async Task<IResult> StartWorkTask(StartWorkTaskInput command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(command, cancellationToken);

		return result.Match<IResult>(
			workTask => TypedResults.CreatedAtRoute($"{OperationIdPrefix}{ApiConstants.GetByIdRoute}", new { id = result }),
			validationException => TypedResults.BadRequest(validationException.Message));
	}

	private static async Task<IResult> GetWorkTaskById(Guid id, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(new WorkTaskInput(id), cancellationToken);
		return result.Match<IResult>(
			workTask => TypedResults.Ok(workTask),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> UpdateWorkTask([FromRoute] Guid id, UpdateWorkTaskCommandDto command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(
			new UpdateWorkTaskInput(
				Id: id,
				ProjectId: command.ProjectId,
				Description: command.Description,
				Details: command.Details,
				Date: command.Date,
				Start: command.Start,
				End: command.End),
			cancellationToken);

		return result.Match<IResult>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> PatchWorkTask([FromRoute] Guid id, PatchWorkTaskCommandDto command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(
			new PatchWorkTaskInput(
				Id: id,
				ProjectId: command.ProjectId,
				Description: command.Description,
				Details: command.Details,
				Date: command.Date,
				Start: command.Start,
				End: command.End),
		cancellationToken);

		return result.Match<IResult>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> DeleteWorkTask([FromRoute] Guid id, IExecutor executor, CancellationToken cancellationToken)
	{
		await executor.ExecuteCommand(new DeleteWorkTaskInput(id), cancellationToken);
		return TypedResults.NoContent();
	}
}
