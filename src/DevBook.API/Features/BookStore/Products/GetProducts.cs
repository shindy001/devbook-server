using DevBook.API.Helpers;

namespace DevBook.API.Features.BookStore.Products;

public sealed record GetProductsQuery(
	int? PageSize = null,
	int? Offset = null,
	ProductType? ProductType = null,
	Guid? ProductCategoryId = null)
	: IQuery<IEnumerable<Product>>;

internal sealed class GetProductsQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetProductsQuery, IEnumerable<Product>>
{
	public async Task<IEnumerable<Product>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
	{
		IQueryable<Product> data = dbContext.Products;

		if (query.ProductType != null)
		{
			data = data.Where(x => x.ProductType == query.ProductType);
		}

		if (query.ProductCategoryId != null)
		{
			data = data.Where(x => x.ProductCategoryIds.Contains(query.ProductCategoryId.Value));
		}

		return await data
			.Skip(PagingHelper.NormalizeOffset(query.Offset))
			.Take(PagingHelper.NormalizePageSize(query.PageSize))
			.ToListAsync(cancellationToken);
	}
}
