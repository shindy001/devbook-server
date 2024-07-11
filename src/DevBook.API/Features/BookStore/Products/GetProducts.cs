using DevBook.API.Helpers;

namespace DevBook.API.Features.BookStore.Products;

internal sealed record GetProductsQuery(int? PageSize, int? ItemLimit) : IQuery<IEnumerable<Product>>;

internal sealed class GetProductsQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetProductsQuery, IEnumerable<Product>>
{
	public async Task<IEnumerable<Product>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
	{
		return await dbContext.Products
			.OrderBy(x => x.Name)
			.Skip(PagingHelper.NormalizeItemLimit(query.ItemLimit))
			.Take(PagingHelper.NormalizePageSize(query.PageSize))
			.ToListAsync(cancellationToken);
	}
}
