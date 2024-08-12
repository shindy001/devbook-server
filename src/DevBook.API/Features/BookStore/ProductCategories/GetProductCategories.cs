using DevBook.API.Helpers;

namespace DevBook.API.Features.BookStore.ProductCategories;

public sealed record GetProductCategoriesQuery(int? PageSize, int? Offset) : IQuery<IEnumerable<ProductCategoryDto>>;

internal sealed class GetProductCategoriesQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetProductCategoriesQuery, IEnumerable<ProductCategoryDto>>
{
	public async Task<IEnumerable<ProductCategoryDto>> Handle(GetProductCategoriesQuery query, CancellationToken cancellationToken)
	{
		var productCategories = await dbContext.ProductCategories
			.OrderBy(x => x.Name)
			.Skip(PagingHelper.NormalizeOffset(query.Offset))
			.Take(PagingHelper.NormalizePageSize(query.PageSize))
			.ToListAsync(cancellationToken);

		var subcategoryIds = productCategories.SelectMany(x => x.Subcategories).Distinct();
		IEnumerable<ProductCategory> subcategories = [];
		if (subcategoryIds.Any())
		{
			subcategories = await dbContext.ProductCategories
				.Where(x => subcategoryIds.Contains(x.Id))
				.ToListAsync(cancellationToken);
		}

		return productCategories.Select(x => x.ToDto(subcategories));
	}
}
