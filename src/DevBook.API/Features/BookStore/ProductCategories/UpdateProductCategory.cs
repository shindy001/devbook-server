namespace DevBook.API.Features.BookStore.ProductCategories;

public sealed record UpdateProductCategoryCommandDto() : ICommand<OneOf<Success, NotFound>>
{
	[Required]
	public required string Name { get; init; }
	public required bool IsTopLevelCategory { get; init; }
	public required IEnumerable<Guid> Subcategories { get; init; }
}

public sealed record UpdateProductCategoryCommand(
	Guid Id,
	string Name,
	bool IsTopLevelCategory,
	IEnumerable<Guid> Subcategories)
	: ICommand<OneOf<Success, NotFound>>;

public sealed class UpdateProductCategoryCommandValidator : AbstractValidator<UpdateProductCategoryCommand>
{
	public UpdateProductCategoryCommandValidator()
	{
		RuleFor(x => x.Id).NotEqual(Guid.Empty);
		RuleFor(x => x.Name).NotEmpty();
	}
}

internal sealed class UpdateProductCategoryCommandHandler(DevBookDbContext dbContext) : ICommandHandler<UpdateProductCategoryCommand, OneOf<Success, NotFound>>
{
	public async Task<OneOf<Success, NotFound>> Handle(UpdateProductCategoryCommand command, CancellationToken cancellationToken)
	{
		var productCategory = await dbContext.ProductCategories.FindAsync([command.Id], cancellationToken);
		if (productCategory is null)
		{
			return new NotFound();
		}

		await ValidateSubcategories(productCategory.Id, command.Subcategories, cancellationToken);

		var update = new Dictionary<string, object?>()
		{
			[nameof(ProductCategory.Name)] = command.Name,
			[nameof(ProductCategory.IsTopLevelCategory)] = command.IsTopLevelCategory,
			[nameof(ProductCategory.Subcategories)] = command.Subcategories,
		};

		dbContext.ProductCategories.Entry(productCategory).CurrentValues.SetValues(update);
		await dbContext.SaveChangesAsync(cancellationToken);
		return new Success();
	}

	private async Task ValidateSubcategories(Guid currentCategoryId, IEnumerable<Guid> subcategories, CancellationToken cancellationToken)
	{
		if (subcategories.Contains(currentCategoryId))
		{
			throw new DevBookValidationException(nameof(UpdateProductCategoryCommand.Subcategories), $"Subcategories cannot contain Id of current product category to update '{currentCategoryId}'.");
		}

		if (subcategories.Any() == true)
		{
			await ProductCategoryHelper.EnsureProductCategoriesExist(subcategories, dbContext, cancellationToken);
		}
	}
}
