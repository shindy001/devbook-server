namespace DevBook.API.Features.BookStore.Authors;

internal sealed record DeleteAuthorCommand(Guid Id) : ICommand;

internal class DeleteAuthorCommandHandler(DevBookDbContext dbContext) : ICommandHandler<DeleteAuthorCommand>
{
	public async Task Handle(DeleteAuthorCommand command, CancellationToken cancellationToken)
	{
		var author = await dbContext.Authors.FindAsync([command.Id], cancellationToken);

		if (author is not null)
		{
			dbContext.Authors.Remove(author);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
