using DevBook.API.Features.BookStore.Shared;

namespace DevBook.API.Features.BookStore.Products.ProductCategories;

public sealed record UpdateProductCategoryCommandDto() : ICommand<OneOf<Success, NotFound>>
{
	[Required]
	public required string Name { get; init; }
}

public sealed record UpdateProductCategoryCommand(
	Guid Id,
	string Name)
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
		else
		{
			var update = new Dictionary<string, object?>()
			{
				[nameof(ProductCategory.Name)] = command.Name,
			};

			dbContext.ProductCategories.Entry(productCategory).CurrentValues.SetValues(update);
			await dbContext.SaveChangesAsync(cancellationToken);
			return new Success();
		}
	}
}
