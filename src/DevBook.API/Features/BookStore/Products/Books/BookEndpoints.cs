namespace DevBook.API.Features.BookStore.Products.Books;

internal static class BookEndpoints
{
	private const string OperationIdPrefix = "Books.";

	public static RouteGroupBuilder MapBookEndpoints(this RouteGroupBuilder groupBuilder)
	{
		groupBuilder.MapPost("/", CreateBook)
			.WithName($"{OperationIdPrefix}Create")
			.Produces(StatusCodes.Status201Created);

		groupBuilder.MapPut("/{id:guid}", UpdateBook)
			.WithName($"{OperationIdPrefix}Update")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapPatch("/{id:guid}", PatchBook)
			.WithName($"{OperationIdPrefix}Patch")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

		return groupBuilder;
	}

	private static async Task<IResult> CreateBook(CreateBookCommand command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(command, cancellationToken);
		return TypedResults.CreatedAtRoute($"{ApiConstants.ProductsOperationIdPrefix}{ApiConstants.GetByIdRoute}", new { id = result.Id });
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
				ProductCategoryIds: command.ProductCategoryIds),
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
				ProductCategoryIds: command.ProductCategoryIds),
		cancellationToken);

		return result.Match<IResult>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound(id));
	}
}
