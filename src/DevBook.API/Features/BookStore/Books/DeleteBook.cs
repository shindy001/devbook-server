namespace DevBook.API.Features.BookStore.Books;

internal sealed record DeleteBookCommand(Guid Id) : ICommand;

internal class DeleteBookCommandHandler(DevBookDbContext dbContext) : ICommandHandler<DeleteBookCommand>
{
	public async Task Handle(DeleteBookCommand command, CancellationToken cancellationToken)
	{
		var book = await dbContext.Books.FindAsync([command.Id], cancellationToken);

		if (book is not null)
		{
			dbContext.Books.Remove(book);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
