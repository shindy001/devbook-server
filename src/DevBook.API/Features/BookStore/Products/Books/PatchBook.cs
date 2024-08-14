using DevBook.API.Features.BookStore.ProductCategories;

namespace DevBook.API.Features.BookStore.Products.Books;

public record PatchBookCommandDto(
	string? Name = null,
	decimal? RetailPrice = null,
	decimal? Price = null,
	decimal? DiscountAmmount = null,
	string? Author = null,
	string? Description = null,
	string? CoverImageUrl = null,
	IList<Guid>? ProductCategoryIds = null)
	: ICommand<OneOf<Success, NotFound>>;

public record PatchBookCommand(
	Guid Id,
	string? Name,
	decimal? RetailPrice,
	decimal? Price,
	decimal? DiscountAmmount,
	string? Author,
	string? Description,
	string? CoverImageUrl,
	IList<Guid>? ProductCategoryIds)
	: ICommand<OneOf<Success, NotFound>>;

public sealed class PatchBookCommandValidator : AbstractValidator<PatchBookCommand>
{
	public PatchBookCommandValidator()
	{
		RuleFor(x => x.Id).NotEqual(Guid.Empty);
		When(x => x.RetailPrice is not null, () => RuleFor(x => x.RetailPrice).GreaterThan(decimal.Zero));
		When(x => x.Price is not null, () => RuleFor(x => x.Price).GreaterThan(decimal.Zero));
		When(x => x.DiscountAmmount is not null, () => RuleFor(x => x.DiscountAmmount).GreaterThanOrEqualTo(decimal.Zero));
	}
}

internal sealed class PatchBookCommandHandler(DevBookDbContext dbContext) : ICommandHandler<PatchBookCommand, OneOf<Success, NotFound>>
{
	public async Task<OneOf<Success, NotFound>> Handle(PatchBookCommand command, CancellationToken cancellationToken)
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
			[nameof(Book.Name)] = command.Name ?? book.Name,
			[nameof(Book.RetailPrice)] = command.RetailPrice ?? book.RetailPrice,
			[nameof(Book.Price)] = command.Price ?? book.Price,
			[nameof(Book.DiscountAmmount)] = command.DiscountAmmount ?? book.DiscountAmmount,
			[nameof(Book.Author)] = command.Author ?? book.Author,
			[nameof(Book.Description)] = command.Description ?? book.Description,
			[nameof(Book.CoverImageUrl)] = command.CoverImageUrl ?? book.CoverImageUrl,
			[nameof(Book.ProductCategoryIds)] = command.ProductCategoryIds ?? book.ProductCategoryIds,
		};

		dbContext.Products.Entry(book).CurrentValues.SetValues(update);
		await dbContext.SaveChangesAsync(cancellationToken);
		return new Success();
	}
}
