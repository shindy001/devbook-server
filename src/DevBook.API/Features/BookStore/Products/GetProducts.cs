using DevBook.API.Helpers;

namespace DevBook.API.Features.BookStore.Products;

public sealed record GetProductsQuery(
	int? PageSize = null,
	int? Offset = null,
	Guid? ProductCategoryId = null)
	: IQuery<IEnumerable<Product>>;

internal sealed class GetProductsQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetProductsQuery, IEnumerable<Product>>
{
	public async Task<IEnumerable<Product>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
	{
		return query.ProductCategoryId is null
			? await dbContext.Products
				.OrderBy(x => x.Name)
				.Skip(PagingHelper.NormalizeOffset(query.Offset))
				.Take(PagingHelper.NormalizePageSize(query.PageSize))
				.ToListAsync(cancellationToken)
			: await dbContext.Products
				.Where(x => x.ProductCategoryIds.Contains(query.ProductCategoryId.Value))
				.OrderBy(x => x.Name)
				.Skip(PagingHelper.NormalizeOffset(query.Offset))
				.Take(PagingHelper.NormalizePageSize(query.PageSize))
				.ToListAsync(cancellationToken);
	}
}
