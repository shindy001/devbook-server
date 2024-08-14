using DevBook.API.Features.BookStore.ProductCategories;

namespace DevBook.API.Features.BookStore.Products.Books;

public sealed record UpdateBookCommandDto() : ICommand<OneOf<Success, NotFound>>
{
	[Required]
	public required string Name { get; init; }

	[Required]
	public required decimal RetailPrice { get; init; }

	[Required]
	public required decimal Price { get; init; }

	[Required]
	public required decimal DiscountAmmount { get; init; }

	public string? Author { get; init; }
	public string? Description { get; init; }
	public string? CoverImageUrl { get; init; }
	public IList<Guid>? ProductCategoryIds { get; init; }
}

public sealed record UpdateBookCommand(
	Guid Id,
	string Name,
	decimal RetailPrice,
	decimal Price,
	decimal DiscountAmmount,
	string? Author,
	string? Description,
	string? CoverImageUrl,
	IList<Guid>? ProductCategoryIds)
	: ICommand<OneOf<Success, NotFound>>;

public sealed class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
	public UpdateBookCommandValidator()
	{
		RuleFor(x => x.Id).NotEqual(Guid.Empty);
		RuleFor(x => x.Name).NotEmpty();
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

		if (command.ProductCategoryIds?.Any() == true)
		{
			await ProductCategoryHelper.EnsureProductCategoriesExist(command.ProductCategoryIds, dbContext, cancellationToken);
		}

		var update = new Dictionary<string, object?>()
		{
			[nameof(Book.Name)] = command.Name,
			[nameof(Book.RetailPrice)] = command.RetailPrice,
			[nameof(Book.Price)] = command.Price,
			[nameof(Book.DiscountAmmount)] = command.DiscountAmmount,
			[nameof(Book.Author)] = command.Author,
			[nameof(Book.Description)] = command.Description,
			[nameof(Book.CoverImageUrl)] = command.CoverImageUrl,
			[nameof(Book.ProductCategoryIds)] = command.ProductCategoryIds,
		};

		dbContext.Products.Entry(book).CurrentValues.SetValues(update);
		await dbContext.SaveChangesAsync(cancellationToken);
		return new Success();
	}
}
