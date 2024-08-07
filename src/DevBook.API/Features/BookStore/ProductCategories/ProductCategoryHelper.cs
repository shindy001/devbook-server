﻿using DevBook.API.Features.BookStore.Products;

namespace DevBook.API.Features.BookStore.ProductCategories;

internal static class ProductCategoryHelper
{
	public static async Task EnsureProductCategoriesExist(IEnumerable<Guid> productCategoriesIds, DevBookDbContext dbContext, CancellationToken cancellationToken)
	{
		foreach (var productCategoryId in productCategoriesIds)
		{
			var category = await dbContext.ProductCategories.FindAsync([productCategoryId], cancellationToken: cancellationToken);
			if (category is null)
			{
				throw new DevBookValidationException(nameof(Product.ProductCategoryIds), $"ProductCategory with ID '{productCategoryId}' not found.");
			}
		}
	}
}
