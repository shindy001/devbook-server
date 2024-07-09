namespace DevBook.API.Features.BookStore.Books;

internal sealed record DeleteBookCommand(Guid Id) : ICommand;

internal class DeleteBookCommandHandler(DevBookDbContext dbContext) : ICommandHandler<DeleteBookCommand>
{
	public async Task Handle(DeleteBookCommand command, CancellationToken cancellationToken)
	{
		var product = await dbContext.Products.FindAsync([command.Id], cancellationToken);

		if (product is not null && product is Book)
		{
			dbContext.Products.Remove(product);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
