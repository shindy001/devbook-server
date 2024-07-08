namespace DevBook.API.Features.BookStore.Books;

internal static class BookEndpoints
{
	private const string OperationIdPrefix = "Books.";
	private const string GetByIdRoute = "GetById";

	public static RouteGroupBuilder MapBookEndpoints(this RouteGroupBuilder groupBuilder)
	{
		groupBuilder.MapGet("/", GetBooks)
			.WithName($"{OperationIdPrefix}GetAll")
			.Produces<IList<Book>>()
			.AllowAnonymous();

		groupBuilder.MapPost("/", CreateBook)
			.WithName($"{OperationIdPrefix}Create")
			.Produces(StatusCodes.Status201Created);

		groupBuilder.MapGet("/{id:guid}", GetBookById)
			.WithName($"{OperationIdPrefix}{GetByIdRoute}")
			.Produces<Book>()
			.Produces(StatusCodes.Status404NotFound)
			.AllowAnonymous();

		groupBuilder.MapPut("/{id:guid}", UpdateBook)
			.WithName($"{OperationIdPrefix}Update")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapPatch("/{id:guid}", PatchBook)
			.WithName($"{OperationIdPrefix}Patch")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapDelete("/{id:guid}", DeleteBook)
			.WithName($"{OperationIdPrefix}Delete")
			.Produces(StatusCodes.Status204NoContent);

		return groupBuilder;
	}

	private static async Task<IResult> GetBooks(int? pageSize, int? itemLimit, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(new GetBooksQuery(PageSize: pageSize, ItemLimit: itemLimit), cancellationToken);
		return TypedResults.Ok(result);
	}

	private static async Task<IResult> CreateBook(CreateBookCommand command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(command, cancellationToken);
		return TypedResults.CreatedAtRoute($"{OperationIdPrefix}{GetByIdRoute}", new { id = result.Id });
	}

	private static async Task<IResult> GetBookById(Guid id, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(new GetBookQuery(id), cancellationToken);
		return result.Match<IResult>(
			book => TypedResults.Ok(book),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> UpdateBook([FromRoute] Guid id, UpdateBookCommandDto command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(
			new UpdateBookCommand(
				Id: id,
				Name: command.Name,
				AuthorId: command.AuthorId,
				RetailPrice: command.RetailPrice,
				Price: command.Price,
				DiscountAmmount: command.DiscountAmmount,
				Description: command.Description,
				CoverImageUrl: command.CoverImageUrl,
				BookCategories: command.BookCategories),
			cancellationToken);

		return result.Match<IResult>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> PatchBook([FromRoute] Guid id, PatchBookCommandDto command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(
			new PatchBookCommand(
				Id: id,
				Name: command.Name,
				AuthorId: command.AuthorId,
				RetailPrice: command.RetailPrice,
				Price: command.Price,
				DiscountAmmount: command.DiscountAmmount,
				Description: command.Description,
				CoverImageUrl: command.CoverImageUrl,
				BookCategories: command.BookCategories),
		cancellationToken);

		return result.Match<IResult>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> DeleteBook([FromRoute] Guid id, IExecutor executor, CancellationToken cancellationToken)
	{
		await executor.ExecuteCommand(new DeleteBookCommand(id), cancellationToken);
		return TypedResults.NoContent();
	}
}
