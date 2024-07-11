﻿using DevBook.API.Helpers;

namespace DevBook.API.Features.BookStore.Products;

internal sealed record GetProductsQuery(int? PageSize, int? Offset, ProductType? ProductType) : IQuery<IEnumerable<Product>>;

internal sealed class GetProductsQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetProductsQuery, IEnumerable<Product>>
{
	public async Task<IEnumerable<Product>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
	{
		return query.ProductType is not null
			? await dbContext.Products
				.Where(x => x.ProductType == query.ProductType)
				.OrderBy(x => x.Name)
				.Skip(PagingHelper.NormalizeOffset(query.Offset))
				.Take(PagingHelper.NormalizePageSize(query.PageSize))
				.ToListAsync(cancellationToken)
			: await dbContext.Products
				.OrderBy(x => x.Name)
				.Skip(PagingHelper.NormalizeOffset(query.Offset))
				.Take(PagingHelper.NormalizePageSize(query.PageSize))
				.ToListAsync(cancellationToken);


	}
}
