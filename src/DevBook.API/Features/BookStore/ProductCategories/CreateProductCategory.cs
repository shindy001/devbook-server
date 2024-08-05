namespace DevBook.API.Features.BookStore.ProductCategories;

public sealed record CreateProductCategoryCommand : ICommand<ProductCategory>
{
	[Required]
	public required string Name { get; init; }
	public bool? IsTopLevelCategory { get; init; }
	public IEnumerable<Guid>? Subcategories { get; init; }
}

public sealed class CreateProductCategoryCommandValidator : AbstractValidator<CreateProductCategoryCommand>
{
	public CreateProductCategoryCommandValidator()
	{
		RuleFor(x => x.Name).NotEmpty();
	}
}

internal sealed class CreateProductCategoryCommandHandler(DevBookDbContext dbContext) : ICommandHandler<CreateProductCategoryCommand, ProductCategory>
{
	public async Task<ProductCategory> Handle(CreateProductCategoryCommand command, CancellationToken cancellationToken)
	{
		if (command.Subcategories?.Any() == true)
		{
			await ProductCategoryHelper.EnsureProductCategoriesExist(command.Subcategories, dbContext, cancellationToken);
		}

		var newItem = new ProductCategory
		{
			Name = command.Name,
			IsTopLevelCategory = command.IsTopLevelCategory ?? false,
			Subcategories = command.Subcategories?.ToList() ?? [],
		};

		await dbContext.ProductCategories.AddAsync(newItem, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
		return newItem;
	}
}
