namespace DevBook.API.Features.BookStore.Authors;

public sealed record UpdateAuthorCommandDto(
	string Name,
	string? Description)
	: ICommand<OneOf<Success, NotFound>>;

public sealed record UpdateAuthorCommand(
	Guid Id,
	string Name,
	string? Description)
	: ICommand<OneOf<Success, NotFound>>;

public sealed class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommand>
{
	public UpdateAuthorCommandValidator()
	{
		RuleFor(x => x.Id).NotEqual(Guid.Empty);
		RuleFor(x => x.Name).NotEmpty();
	}
}

internal sealed class UpdateAuthorCommandHandler(DevBookDbContext dbContext) : ICommandHandler<UpdateAuthorCommand, OneOf<Success, NotFound>>
{
	public async Task<OneOf<Success, NotFound>> Handle(UpdateAuthorCommand command, CancellationToken cancellationToken)
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
				[nameof(Author.Name)] = command.Name,
				[nameof(Author.Description)] = command.Description
			};

			dbContext.Authors.Entry(author).CurrentValues.SetValues(update);
			await dbContext.SaveChangesAsync(cancellationToken);
			return new Success();
		}
	}
}
