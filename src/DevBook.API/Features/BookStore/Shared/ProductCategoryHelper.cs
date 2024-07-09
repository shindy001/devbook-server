namespace DevBook.API.Features.BookStore.Shared;

internal static class ProductCategoryHelper
{
	public static async Task EnsureProductCategoriesExist(IEnumerable<ProductCategory> productCategories, DevBookDbContext dbContext, CancellationToken cancellationToken)
	{
		foreach (var productCategory in productCategories)
		{
			var category = await dbContext.ProductCategories.FindAsync([productCategory.Id], cancellationToken: cancellationToken);
			if (category is null)
			{
				throw new DevBookValidationException(nameof(Product.ProductCategories), $"Category '{productCategory.Name}' not found.");
			}
		}
	}
}
