using DevBook.API.Features.BookStore.Shared;
using MediatR;

namespace DevBook.API.Features.BookStore.Books;

public sealed record UpdateBookCommandDto() : ICommand<OneOf<Success, NotFound>>
{
	[Required]
	public required string Name { get; init; }

	[Required]
	public required Guid AuthorId { get; set; }

	[Required]
	public required decimal RetailPrice { get; init; }

	[Required]
	public required decimal Price { get; init; }

	[Required]
	public required decimal DiscountAmmount { get; init; }

	public string? Description { get; init; }
	public string? CoverImageUrl { get; init; }
	public IEnumerable<ProductCategory>? ProductCategories { get; set; }
}

public sealed record UpdateBookCommand(
	Guid Id,
	string Name,
	Guid AuthorId,
	decimal RetailPrice,
	decimal Price,
	decimal DiscountAmmount,
	string? Description,
	string? CoverImageUrl,
	IEnumerable<ProductCategory>? ProductCategories)
	: ICommand<OneOf<Success, NotFound>>;

public sealed class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
	public UpdateBookCommandValidator()
	{
		RuleFor(x => x.Id).NotEqual(Guid.Empty);
		RuleFor(x => x.Name).NotEmpty();
		RuleFor(x => x.AuthorId).NotEqual(Guid.Empty);
		RuleFor(x => x.RetailPrice).GreaterThan(decimal.Zero);
		RuleFor(x => x.Price).GreaterThan(decimal.Zero);
		RuleFor(x => x.DiscountAmmount).GreaterThanOrEqualTo(decimal.Zero);
	}
}

internal sealed class UpdateBookCommandHandler(DevBookDbContext dbContext) : ICommandHandler<UpdateBookCommand, OneOf<Success, NotFound>>
{
	public async Task<OneOf<Success, NotFound>> Handle(UpdateBookCommand command, CancellationToken cancellationToken)
	{
		var book = await dbContext.Products.FindAsync([command.Id], cancellationToken) as Book;
		if (book is null)
		{
			return new NotFound();
		}

		var author = await dbContext.Authors.FindAsync([command.AuthorId], cancellationToken: cancellationToken);
		if (author is null)
		{
			throw new DevBookValidationException(nameof(command.AuthorId), $"AuthorId '{command.AuthorId}' not found.");
		}

		if (command.ProductCategories?.Any() == true)
		{
			await ProductCategoryHelper.EnsureProductCategoriesExist(command.ProductCategories, dbContext, cancellationToken);
		}

		var update = new Dictionary<string, object?>()
		{
			[nameof(Book.Name)] = command.Name,
			[nameof(Book.AuthorId)] = command.AuthorId,
			[nameof(Book.RetailPrice)] = command.RetailPrice,
			[nameof(Book.Price)] = command.Price,
			[nameof(Book.DiscountAmmount)] = command.DiscountAmmount,
			[nameof(Book.Description)] = command.Description,
			[nameof(Book.CoverImageUrl)] = command.CoverImageUrl,
			[nameof(Book.ProductCategories)] = command.ProductCategories,
		};

		dbContext.Products.Entry(book).CurrentValues.SetValues(update);
		await dbContext.SaveChangesAsync(cancellationToken);
		return new Success();
	}
}
