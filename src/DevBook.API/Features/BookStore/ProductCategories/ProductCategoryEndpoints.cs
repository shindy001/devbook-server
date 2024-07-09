namespace DevBook.API.Features.BookStore.ProductCategories;

internal static class ProductCategoryEndpoints
{
	private const string OperationIdPrefix = "ProductCategories.";

	public static RouteGroupBuilder MapProductCategoryEndpoints(this RouteGroupBuilder groupBuilder)
	{
		groupBuilder.MapGet("/", GetProductCategories)
			.WithName($"{OperationIdPrefix}GetAll")
			.Produces<IList<ProductCategory>>()
			.AllowAnonymous();

		groupBuilder.MapPost("/", CreateProductCategory)
			.WithName($"{OperationIdPrefix}Create")
			.Produces(StatusCodes.Status201Created);

		groupBuilder.MapGet("/{id:guid}", GetProductCategoryById)
			.WithName($"{OperationIdPrefix}{ApiConstants.GetByIdRoute}")
			.Produces<ProductCategory>()
			.Produces(StatusCodes.Status404NotFound)
			.AllowAnonymous();

		groupBuilder.MapPut("/{id:guid}", UpdateProductCategory)
			.WithName($"{OperationIdPrefix}Update")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapDelete("/{id:guid}", DeleteProductCategory)
			.WithName($"{OperationIdPrefix}Delete")
			.Produces(StatusCodes.Status204NoContent);

		return groupBuilder;
	}

	private static async Task<IResult> GetProductCategories(int? pageSize, int? itemLimit, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(new GetProductCategoriesQuery(PageSize: pageSize, ItemLimit: itemLimit), cancellationToken);
		return TypedResults.Ok(result);
	}

	private static async Task<IResult> CreateProductCategory(CreateProductCategoryCommand command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(command, cancellationToken);
		return TypedResults.CreatedAtRoute($"{OperationIdPrefix}{ApiConstants.GetByIdRoute}", new { id = result.Id });
	}

	private static async Task<IResult> GetProductCategoryById(Guid id, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(new GetProductCategoryQuery(id), cancellationToken);
		return result.Match<IResult>(
			productCategory => TypedResults.Ok(productCategory),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> UpdateProductCategory([FromRoute] Guid id, UpdateProductCategoryCommandDto command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(
			new UpdateProductCategoryCommand(
				Id: id,
				Name: command.Name),
			cancellationToken);

		return result.Match<IResult>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> DeleteProductCategory([FromRoute] Guid id, IExecutor executor, CancellationToken cancellationToken)
	{
		await executor.ExecuteCommand(new DeleteProductCategoryCommand(id), cancellationToken);
		return TypedResults.NoContent();
	}
}
