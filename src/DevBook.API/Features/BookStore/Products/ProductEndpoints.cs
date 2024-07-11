namespace DevBook.API.Features.BookStore.Products;

internal static class ProductEndpoints
{
	private const string OperationIdPrefix = "Products.";

	public static RouteGroupBuilder MapProductEndpoints(this RouteGroupBuilder groupBuilder)
	{
		groupBuilder.MapGet("/", GetProducts)
			.WithName($"{OperationIdPrefix}GetAll")
			.Produces<IList<Product>>()
			.AllowAnonymous();

		groupBuilder.MapGet("/{id:guid}", GetProductById)
			.WithName($"{OperationIdPrefix}{ApiConstants.GetByIdRoute}")
			.Produces<Product>()
			.Produces(StatusCodes.Status404NotFound)
			.AllowAnonymous();

		groupBuilder.MapDelete("/{id:guid}", DeleteProduct)
			.WithName($"{OperationIdPrefix}Delete")
			.Produces(StatusCodes.Status204NoContent);

		return groupBuilder;
	}

	private static async Task<IResult> GetProducts(int? pageSize, int? itemLimit, ProductType? productType, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(new GetProductsQuery(PageSize: pageSize, Offset: itemLimit, ProductType: productType), cancellationToken);
		return TypedResults.Ok(result);
	}

	private static async Task<IResult> GetProductById(Guid id, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(new GetProductQuery(id), cancellationToken);
		return result.Match<IResult>(
			book => TypedResults.Ok(book),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> DeleteProduct([FromRoute] Guid id, IExecutor executor, CancellationToken cancellationToken)
	{
		await executor.ExecuteCommand(new DeleteProductCommand(id), cancellationToken);
		return TypedResults.NoContent();
	}
}
