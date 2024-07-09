namespace DevBook.API.Features.BookStore.ProductCategories;

internal sealed record DeleteProductCategoryCommand(Guid Id) : ICommand;

internal class DeleteProductCategoryCommandHandler(DevBookDbContext dbContext) : ICommandHandler<DeleteProductCategoryCommand>
{
	public async Task Handle(DeleteProductCategoryCommand command, CancellationToken cancellationToken)
	{
		var productCategory = await dbContext.ProductCategories.FindAsync([command.Id], cancellationToken);

		// Check for books with this category and throw DevBookError when there are books in this category => cannot delete category with products
		if (productCategory is not null)
		{
			dbContext.ProductCategories.Remove(productCategory);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
