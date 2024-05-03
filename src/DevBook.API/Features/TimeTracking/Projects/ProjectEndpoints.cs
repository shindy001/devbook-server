namespace DevBook.API.Features.TimeTracking.Projects;

internal static class ProjectEndpoints
{
	private const string OperationIdPrefix = "Projects.";
	private const string GetByIdRoute = "GetById";

	public static RouteGroupBuilder MapProjectEndpoints(this RouteGroupBuilder groupBuilder)
	{
		groupBuilder.MapGet("/", GetProjects)
			.WithName($"{OperationIdPrefix}GetAll")
			.Produces<IList<ProjectDto>>();

		groupBuilder.MapPost("/", CreateProject)
			.WithName($"{OperationIdPrefix}Create")
			.Produces(StatusCodes.Status201Created);

		groupBuilder.MapGet("/{id:guid}", GetProjectById)
			.WithName($"{OperationIdPrefix}{GetByIdRoute}")
			.Produces<ProjectDto>()
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapPut("/{id:guid}", UpdateProject)
			.WithName($"{OperationIdPrefix}Update")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapPatch("/{id:guid}", PatchProject)
			.WithName($"{OperationIdPrefix}Patch")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapDelete("/{id:guid}", DeleteProject)
			.WithName($"{OperationIdPrefix}Delete")
			.Produces(StatusCodes.Status204NoContent);

		return groupBuilder;
	}

	private static async Task<IResult> GetProjects(IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(new GetProjectsQuery(), cancellationToken);
		return TypedResults.Ok(result);
	}

	private static async Task<IResult> CreateProject(CreateProjectInput command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(command, cancellationToken);
		return TypedResults.CreatedAtRoute($"{OperationIdPrefix}{GetByIdRoute}", new { id = result.Id });
	}

	private static async Task<IResult> GetProjectById(Guid id, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(new GetProjectInput(id), cancellationToken);
		return result.Match<IResult>(
			project => TypedResults.Ok(project),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> UpdateProject([FromRoute] Guid id, UpdateProjectCommandDto command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(
			new UpdateProjectInput(
				Id: id,
				Name: command.Name,
				Details: command.Details,
				HourlyRate: command.HourlyRate,
				Currency: command.Currency,
				HexColor: command.HexColor),
			cancellationToken);

		return result.Match<IResult>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> PatchProject([FromRoute] Guid id, PatchProjectCommandDto command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(
			new PatchProjectInput(
				Id: id,
				Name: command.Name,
				Details: command.Details,
				HourlyRate: command.HourlyRate,
				Currency: command.Currency,
				HexColor: command.HexColor),
		cancellationToken);

		return result.Match<IResult>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> DeleteProject([FromRoute] Guid id, IExecutor executor, CancellationToken cancellationToken)
	{
		await executor.ExecuteCommand(new DeleteProjectInput(id), cancellationToken);
		return TypedResults.NoContent();
	}
}
