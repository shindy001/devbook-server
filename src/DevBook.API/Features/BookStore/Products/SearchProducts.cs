using DevBook.API.Helpers;

namespace DevBook.API.Features.BookStore.Products;

public sealed record SearchProductsQuery(
	string SearchTerm)
	: IQuery<IEnumerable<Product>>;

internal sealed class SearchProductsQueryHandler(DevBookDbContext dbContext) : IQueryHandler<SearchProductsQuery, IEnumerable<Product>>
{
	public async Task<IEnumerable<Product>> Handle(SearchProductsQuery query, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(query.SearchTerm)) {
			return [];
		}

		IQueryable<Product> data = dbContext.Products.Where(x => EF.Functions.Like(x.Name, $"%{query.SearchTerm}%"));

		return await data
			.Take(PagingHelper.NormalizePageSize(ApiConstants.MaxPageSize))
			.ToListAsync(cancellationToken);
	}
}
