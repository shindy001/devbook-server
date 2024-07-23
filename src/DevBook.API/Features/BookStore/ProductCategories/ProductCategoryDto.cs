namespace DevBook.API.Features.BookStore.ProductCategories;

public sealed record ProductCategoryDto
{
	[Required]
	public required Guid Id { get; init; }

	[Required]
	public required string Name { get; init; }
}

public static class ProductCategoryMappings
{
	public static ProductCategoryDto ToDto(this ProductCategory productCategory)
	{
		return new ProductCategoryDto
		{
			Id = productCategory.Id,
			Name = productCategory.Name,
		};
	}
}
