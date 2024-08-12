namespace DevBook.API.Features.BookStore.ProductCategories;

public sealed record ProductCategoryDto
{
	[Required]
	public required Guid Id { get; init; }

	[Required]
	public required string Name { get; init; }

	public bool IsTopLevelCategory { get; init; }

	public IList<string>? Subcategories { get; init; }
}

public static class ProductCategoryMappings
{
	public static ProductCategoryDto ToDto(this ProductCategory productCategory, IEnumerable<ProductCategory>? subcategories)
	{
		var subcategoryNames = subcategories?
			.Where(x => productCategory.Subcategories.Contains(x.Id))
			.Select(x => x.Name)
			.ToList();

		return new ProductCategoryDto
		{
			Id = productCategory.Id,
			Name = productCategory.Name,
			IsTopLevelCategory = productCategory.IsTopLevelCategory,
			Subcategories = subcategoryNames,
		};
	}
}
