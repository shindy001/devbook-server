namespace DevBook.API.Features.BookStore.ProductCategories;

public sealed record DeleteProductCategoryCommand(Guid Id) : ICommand;

internal class DeleteProductCategoryCommandHandler(DevBookDbContext dbContext) : ICommandHandler<DeleteProductCategoryCommand>
{
	public async Task Handle(DeleteProductCategoryCommand command, CancellationToken cancellationToken)
	{
		var productCategory = await dbContext.ProductCategories.FindAsync([command.Id], cancellationToken);

		var productWithCategory = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductCategoryIds.Contains(command.Id));
		if (productWithCategory is not null)
		{
			throw new DevBookValidationException(
				nameof(command.Id),
				$"ProductCategory with id '{command.Id}' is used on some Products and cannot be deleted. To delete ProductCategory, you need to remove it from Products first.");
		}

		if (productCategory is not null)
		{
			dbContext.ProductCategories.Remove(productCategory);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
