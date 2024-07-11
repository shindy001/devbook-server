﻿using DevBook.API.Features.BookStore.Shared;

namespace DevBook.API.Features.BookStore.Products.ProductCategories;

internal sealed record GetProductCategoryQuery(Guid Id) : IQuery<OneOf<ProductCategory, NotFound>>;

internal class GetProductCategoryQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetProductCategoryQuery, OneOf<ProductCategory, NotFound>>
{
	public async Task<OneOf<ProductCategory, NotFound>> Handle(GetProductCategoryQuery query, CancellationToken cancellationToken)
	{
		var productCategory = await dbContext.ProductCategories.FindAsync([query.Id], cancellationToken);

		return productCategory is null
			? new NotFound()
			: productCategory;
	}
}