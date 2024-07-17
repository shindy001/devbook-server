using DevBook.API.Helpers;

namespace DevBook.API.Features.BookStore.ProductCategories;

internal sealed record GetProductCategoriesQuery(int? PageSize, int? Offset) : IQuery<IEnumerable<ProductCategory>>;

internal sealed class GetProductCategoriesQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetProductCategoriesQuery, IEnumerable<ProductCategory>>
{
	public async Task<IEnumerable<ProductCategory>> Handle(GetProductCategoriesQuery query, CancellationToken cancellationToken)
	{
		return await dbContext.ProductCategories
			.OrderBy(x => x.Name)
			.Skip(PagingHelper.NormalizeOffset(query.Offset))
			.Take(PagingHelper.NormalizePageSize(query.PageSize))
			.ToListAsync(cancellationToken);
	}
}
