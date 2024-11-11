namespace DevBook.API.Features.BookStore.Products;

internal static class ProductEndpoints
{
	public static RouteGroupBuilder MapProductEndpoints(this RouteGroupBuilder groupBuilder)
	{
		groupBuilder.MapGet("/", GetProducts)
			.WithName($"{ApiConstants.ProductsOperationIdPrefix}GetAll")
			.Produces<IList<ProductDto>>()
			.AllowAnonymous();

		groupBuilder.MapGet("/search={term}", SearchProducts)
			.WithName($"{ApiConstants.ProductsOperationIdPrefix}Search")
			.Produces<IList<ProductDto>>()
			.AllowAnonymous();

		groupBuilder.MapGet("/{id:guid}", GetProductById)
			.WithName($"{ApiConstants.ProductsOperationIdPrefix}{ApiConstants.GetByIdRoute}")
			.Produces<ProductDto>()
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
		var dtos = result.Select(x => x.ToDto());
		return TypedResults.Ok(dtos);
	}

	private static async Task<IResult> SearchProducts([FromQuery(Name = "search")] string searchTerm, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(new SearchProductsQuery(searchTerm), cancellationToken);
		var dtos = result.Select(x => x.ToDto());
		return TypedResults.Ok(dtos);
	}

	private static async Task<IResult> GetProductById([AsParameters] GetProductQuery query, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(query, cancellationToken);
		return result.Match<IResult>(
			book => TypedResults.Ok(book.ToDto()),
			notFound => TypedResults.NotFound(query.Id));
	}

	private static async Task<IResult> DeleteProduct([AsParameters] DeleteProductCommand command, IExecutor executor, CancellationToken cancellationToken)
	{
		await executor.ExecuteCommand(command, cancellationToken);
		return TypedResults.NoContent();
	}
}
