﻿using DevBook.API.Features.BookStore.ProductCategories;

namespace DevBook.API.Features.BookStore.Products.Books;

public sealed record CreateBookCommand : ICommand<Book>
{
	[Required]
	public required string Name { get; init; }
	public required Guid AuthorId { get; set; }
	public required decimal RetailPrice { get; init; }
	public required decimal Price { get; init; }
	public required decimal DiscountAmmount { get; init; }

	public string? Description { get; init; }
	public string? CoverImageUrl { get; init; }
	public IList<Guid>? ProductCategoryIds { get; set; }
}

public sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
	public CreateBookCommandValidator()
	{
		RuleFor(x => x.Name).NotEmpty();
		RuleFor(x => x.AuthorId).NotEqual(Guid.Empty);
		RuleFor(x => x.RetailPrice).GreaterThan(decimal.Zero);
		RuleFor(x => x.Price).GreaterThan(decimal.Zero);
		RuleFor(x => x.DiscountAmmount).GreaterThanOrEqualTo(decimal.Zero);
	}
}

internal sealed class CreateBookCommandHandler(DevBookDbContext dbContext) : ICommandHandler<CreateBookCommand, Book>
{
	public async Task<Book> Handle(CreateBookCommand command, CancellationToken cancellationToken)
	{
		var author = await dbContext.Authors.FindAsync([command.AuthorId], cancellationToken: cancellationToken);
		if (author is null)
		{
			throw new DevBookValidationException(nameof(command.AuthorId), $"AuthorId '{command.AuthorId}' not found.");
		}

		if (command.ProductCategoryIds?.Any() == true)
		{
			await ProductCategoryHelper.EnsureProductCategoriesExist(command.ProductCategoryIds, dbContext, cancellationToken);
		}

		var newItem = new Book
		{
			Name = command.Name,
			ProductType = ProductType.Book,
			AuthorId = command.AuthorId,
			RetailPrice = command.RetailPrice,
			Price = command.Price,
			DiscountAmmount = command.DiscountAmmount,
			Description = command.Description,
			CoverImageUrl = command.CoverImageUrl,
			ProductCategoryIds = command.ProductCategoryIds ?? [],
		};
		await dbContext.Products.AddAsync(newItem, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
		return newItem;
	}
}
