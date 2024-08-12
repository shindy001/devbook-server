namespace DevBook.API.Features.BookStore.ProductCategories;

public sealed record GetProductCategoryQuery(Guid Id) : IQuery<OneOf<ProductCategoryDto, NotFound>>;

internal class GetProductCategoryQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetProductCategoryQuery, OneOf<ProductCategoryDto, NotFound>>
{
	public async Task<OneOf<ProductCategoryDto, NotFound>> Handle(GetProductCategoryQuery query, CancellationToken cancellationToken)
	{
		var productCategory = await dbContext.ProductCategories.FindAsync([query.Id], cancellationToken);

		IEnumerable<ProductCategory> subcategories = [];
		if (productCategory?.Subcategories.Any() == true)
		{
			subcategories = await dbContext.ProductCategories
				.Where(x => productCategory.Subcategories.Contains(x.Id))
				.ToListAsync(cancellationToken);
		}

		return productCategory is null
			? new NotFound()
			: productCategory.ToDto(subcategories);
	}
}
