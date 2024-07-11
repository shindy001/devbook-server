﻿using DevBook.API.Helpers;

namespace DevBook.API.Features.BookStore.Products.ProductCategories;

internal sealed record GetProductCategoriesQuery(int? PageSize, int? ItemLimit) : IQuery<IEnumerable<ProductCategory>>;

internal sealed class GetProductCategoriesQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetProductCategoriesQuery, IEnumerable<ProductCategory>>
{
	public async Task<IEnumerable<ProductCategory>> Handle(GetProductCategoriesQuery query, CancellationToken cancellationToken)
	{
		return await dbContext.ProductCategories
			.OrderBy(x => x.Name)
			.Skip(PagingHelper.NormalizeItemLimit(query.ItemLimit))
			.Take(PagingHelper.NormalizePageSize(query.PageSize))
			.ToListAsync(cancellationToken);
	}
}
