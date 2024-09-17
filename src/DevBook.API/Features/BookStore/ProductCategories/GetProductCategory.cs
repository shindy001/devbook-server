namespace DevBook.API.Features.BookStore.ProductCategories;

public sealed record GetProductCategoryQuery(Guid? Id, string? Name) : IQuery<OneOf<ProductCategoryDto, NotFound>>;

public sealed class GetProductCategoryQueryValidator : AbstractValidator<GetProductCategoryQuery>
{
	public GetProductCategoryQueryValidator()
	{
		RuleFor(x => x.Id).NotNull()
			.Unless(x => !string.IsNullOrWhiteSpace(x.Name));
		RuleFor(x => x.Name)
			.NotNull()
			.NotEmpty()
			.Unless(x => x.Id != null);
	}
}

public class GetProductCategoryQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetProductCategoryQuery, OneOf<ProductCategoryDto, NotFound>>
{
	public async Task<OneOf<ProductCategoryDto, NotFound>> Handle(GetProductCategoryQuery query, CancellationToken cancellationToken)
	{
		ProductCategory? productCategory = null;
		if (query.Id != null)
		{
			productCategory = await dbContext.ProductCategories.FindAsync([query.Id], cancellationToken);
		}
		else
		{
			productCategory = await dbContext.ProductCategories.FirstOrDefaultAsync(x => x.Name == query.Name, cancellationToken);
		}

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
