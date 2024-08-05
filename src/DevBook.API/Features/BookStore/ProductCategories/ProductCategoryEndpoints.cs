namespace DevBook.API.Features.BookStore.ProductCategories;

internal static class ProductCategoryEndpoints
{
	private const string OperationIdPrefix = "ProductCategories.";

	public static RouteGroupBuilder MapProductCategoryEndpoints(this RouteGroupBuilder groupBuilder)
	{
		groupBuilder.MapGet("/", GetProductCategories)
			.WithName($"{OperationIdPrefix}GetAll")
			.Produces<IList<ProductCategoryDto>>()
			.AllowAnonymous();

		groupBuilder.MapGet("/{id:guid}", GetProductCategoryById)
			.WithName($"{OperationIdPrefix}{ApiConstants.GetByIdRoute}")
			.Produces<ProductCategoryDto>()
			.Produces(StatusCodes.Status404NotFound)
			.AllowAnonymous();

		groupBuilder.MapPost("/", CreateProductCategory)
			.WithName($"{OperationIdPrefix}Create")
			.Produces(StatusCodes.Status201Created);

		groupBuilder.MapPut("/{id:guid}", UpdateProductCategory)
			.WithName($"{OperationIdPrefix}Update")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapDelete("/{id:guid}", DeleteProductCategory)
			.WithName($"{OperationIdPrefix}Delete")
			.Produces(StatusCodes.Status204NoContent);

		return groupBuilder;
	}

	private static async Task<IResult> GetProductCategories([AsParameters] GetProductCategoriesQuery query, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(query, cancellationToken);
		var dtos = result.Select(x => x.ToDto());
		return TypedResults.Ok(dtos);
	}

	private static async Task<IResult> GetProductCategoryById([AsParameters] GetProductCategoryQuery query, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(query, cancellationToken);
		return result.Match<IResult>(
			productCategory => TypedResults.Ok(productCategory.ToDto()),
			notFound => TypedResults.NotFound(query.Id));
	}

	private static async Task<IResult> CreateProductCategory(CreateProductCategoryCommand command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(command, cancellationToken);
		return TypedResults.CreatedAtRoute($"{OperationIdPrefix}{ApiConstants.GetByIdRoute}", new { id = result.Id });
	}

	private static async Task<IResult> UpdateProductCategory([FromRoute] Guid id, UpdateProductCategoryCommandDto command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(
			new UpdateProductCategoryCommand(
				Id: id,
				Name: command.Name,
				IsTopLevelCategory: command.IsTopLevelCategory,
				Subcategories: command.Subcategories),
			cancellationToken);

		return result.Match<IResult>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> DeleteProductCategory([AsParameters] DeleteProductCategoryCommand command, IExecutor executor, CancellationToken cancellationToken)
	{
		await executor.ExecuteCommand(command, cancellationToken);
		return TypedResults.NoContent();
	}
}
