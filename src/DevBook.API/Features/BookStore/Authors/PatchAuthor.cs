namespace DevBook.API.Features.BookStore.Authors;

public record PatchAuthorCommandDto(
	string? Name = null,
	string? Description = null)
	: ICommand<OneOf<Success, NotFound>>;

public record PatchAuthorCommand(
	Guid Id,
	string? Name,
	string? Description)
	: ICommand<OneOf<Success, NotFound>>;

public sealed class PatchAuthorCommandValidator : AbstractValidator<PatchAuthorCommand>
{
	public PatchAuthorCommandValidator()
	{
		RuleFor(x => x.Id).NotEqual(Guid.Empty);
	}
}

internal sealed class PatchAuthorCommandHandler(DevBookDbContext dbContext) : ICommandHandler<PatchAuthorCommand, OneOf<Success, NotFound>>
{
	public async Task<OneOf<Success, NotFound>> Handle(PatchAuthorCommand command, CancellationToken cancellationToken)
	{
		var author = await dbContext.Authors.FindAsync([command.Id], cancellationToken);
		if (author is null)
		{
			return new NotFound();
		}
		else
		{
			var update = new Dictionary<string, object?>()
			{
				[nameof(Author.Name)] = command.Name ?? author.Name,
				[nameof(Author.Description)] = command.Description ?? author.Description,
			};

			dbContext.Authors.Entry(author).CurrentValues.SetValues(update);
			await dbContext.SaveChangesAsync(cancellationToken);
			return new Success();
		}
	}
}
