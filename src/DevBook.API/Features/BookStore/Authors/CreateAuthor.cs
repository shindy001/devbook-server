﻿namespace DevBook.API.Features.BookStore.Authors;

public sealed record CreateAuthorCommand : ICommand<Author>
{
	[Required]
	public required string Name { get; init; }
	public string? Description { get; init; }
}

public sealed class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
{
	public CreateAuthorCommandValidator()
	{
		RuleFor(x => x.Name).NotEmpty();
	}
}

internal sealed class CreateAuthorCommandHandler(DevBookDbContext dbContext) : ICommandHandler<CreateAuthorCommand, Author>
{
	public async Task<Author> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
	{
		var newItem = new Author { Name = request.Name, Description = request.Description };
		await dbContext.Authors.AddAsync(newItem, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
		return newItem;
	}
}
