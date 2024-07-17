using DevBook.API.Features.BookStore.Products.Books;

namespace DevBook.API.Features.BookStore.Authors;

internal sealed record DeleteAuthorCommand(Guid Id) : ICommand;

internal class DeleteAuthorCommandHandler(DevBookDbContext dbContext) : ICommandHandler<DeleteAuthorCommand>
{
	public async Task Handle(DeleteAuthorCommand command, CancellationToken cancellationToken)
	{
		var author = await dbContext.Authors.FindAsync([command.Id], cancellationToken);

		var productWithAuthor = await dbContext.Products.OfType<Book>().FirstOrDefaultAsync(x => x.AuthorId == command.Id);
		if (productWithAuthor is not null)
		{
			throw new DevBookValidationException(
				nameof(command.Id),
				$"Author with id '{command.Id}' is used on some Book products and cannot be deleted. To delete Author, you need to remove it from products first.");
		}

		if (author is not null)
		{
			dbContext.Authors.Remove(author);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
