namespace DevBook.API.Features.BookStore.Products;

internal sealed record DeleteProductCommand(Guid Id) : ICommand;

internal class DeleteProductCommandHandler(DevBookDbContext dbContext) : ICommandHandler<DeleteProductCommand>
{
	public async Task Handle(DeleteProductCommand command, CancellationToken cancellationToken)
	{
		var product = await dbContext.Products.FindAsync([command.Id], cancellationToken);

		if (product is not null)
		{
			dbContext.Products.Remove(product);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
