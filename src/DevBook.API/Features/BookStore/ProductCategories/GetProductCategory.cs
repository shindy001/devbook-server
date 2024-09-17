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
		else if (query.Name != null)
		{
			// FYI - EFCore does not support equals with StringComparison operator, collation or lower is inefficient but work
			// For better performance consider to setup DB collation on the property 
			productCategory = await dbContext.ProductCategories.FirstOrDefaultAsync(
				x => x.Name.ToLower() == query.Name.ToLower(), cancellationToken);
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
