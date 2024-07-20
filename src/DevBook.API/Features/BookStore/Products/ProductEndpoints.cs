namespace DevBook.API.Features.BookStore.Products;

internal static class ProductEndpoints
{
	public static RouteGroupBuilder MapProductEndpoints(this RouteGroupBuilder groupBuilder)
	{
		groupBuilder.MapGet("/", GetProducts)
			.WithName($"{ApiConstants.ProductsOperationIdPrefix}GetAll")
			.Produces<IList<Product>>()
			.AllowAnonymous();

		groupBuilder.MapGet("/{id:guid}", GetProductById)
			.WithName($"{ApiConstants.ProductsOperationIdPrefix}{ApiConstants.GetByIdRoute}")
			.Produces<Product>()
			.Produces(StatusCodes.Status404NotFound)
			.AllowAnonymous();

		groupBuilder.MapDelete("/{id:guid}", DeleteProduct)
			.WithName($"{ApiConstants.ProductsOperationIdPrefix}Delete")
			.Produces(StatusCodes.Status204NoContent);

		return groupBuilder;
	}

	private static async Task<IResult> GetProducts([AsParameters] GetProductsQuery query, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(query, cancellationToken);
		return TypedResults.Ok(result);
	}

	private static async Task<IResult> GetProductById([AsParameters] GetProductQuery query, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(query, cancellationToken);
		return result.Match<IResult>(
			book => TypedResults.Ok(book),
			notFound => TypedResults.NotFound(query.Id));
	}

	private static async Task<IResult> DeleteProduct([AsParameters] DeleteProductCommand command, IExecutor executor, CancellationToken cancellationToken)
	{
		await executor.ExecuteCommand(command, cancellationToken);
		return TypedResults.NoContent();
	}
}
