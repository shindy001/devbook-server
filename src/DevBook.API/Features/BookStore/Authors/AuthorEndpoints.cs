namespace DevBook.API.Features.BookStore.Authors;

internal static class AuthorEndpoints
{
	private const string OperationIdPrefix = "Authors.";
	private const string GetByIdRoute = "GetById";

	public static RouteGroupBuilder MapAuthorEndpoints(this RouteGroupBuilder groupBuilder)
	{
		groupBuilder.MapGet("/", GetAuthors)
			.WithName($"{OperationIdPrefix}GetAll")
			.Produces<IList<Author>>()
			.AllowAnonymous();

		groupBuilder.MapPost("/", CreateAuthor)
			.WithName($"{OperationIdPrefix}Create")
			.Produces(StatusCodes.Status201Created);

		groupBuilder.MapGet("/{id:guid}", GetAuthorById)
			.WithName($"{OperationIdPrefix}{GetByIdRoute}")
			.Produces<Author>()
			.Produces(StatusCodes.Status404NotFound)
			.AllowAnonymous();

		groupBuilder.MapPut("/{id:guid}", UpdateAuthor)
			.WithName($"{OperationIdPrefix}Update")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapPatch("/{id:guid}", PatchAuthor)
			.WithName($"{OperationIdPrefix}Patch")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapDelete("/{id:guid}", DeleteAuthor)
			.WithName($"{OperationIdPrefix}Delete")
			.Produces(StatusCodes.Status204NoContent);

		return groupBuilder;
	}

	private static async Task<IResult> GetAuthors(int? pageSize, int? itemLimit, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(new GetAuthorsQuery(PageSize: pageSize, ItemLimit: itemLimit), cancellationToken);
		return TypedResults.Ok(result);
	}

	private static async Task<IResult> CreateAuthor(CreateAuthorCommand command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(command, cancellationToken);
		return TypedResults.CreatedAtRoute($"{OperationIdPrefix}{GetByIdRoute}", new { id = result.Id });
	}

	private static async Task<IResult> GetAuthorById(Guid id, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(new GetAuthorQuery(id), cancellationToken);
		return result.Match<IResult>(
			author => TypedResults.Ok(author),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> UpdateAuthor([FromRoute] Guid id, UpdateAuthorCommandDto command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(
			new UpdateAuthorCommand(
				Id: id,
				Name: command.Name,
				Description: command.Description),
			cancellationToken);

		return result.Match<IResult>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> PatchAuthor([FromRoute] Guid id, PatchAuthorCommandDto command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(
			new PatchAuthorCommand(
				Id: id,
				Name: command.Name,
				Description: command.Description),
		cancellationToken);

		return result.Match<IResult>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> DeleteAuthor([FromRoute] Guid id, IExecutor executor, CancellationToken cancellationToken)
	{
		await executor.ExecuteCommand(new DeleteAuthorCommand(id), cancellationToken);
		return TypedResults.NoContent();
	}
}
