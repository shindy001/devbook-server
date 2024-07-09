using DevBook.API.Features.BookStore.Shared;

namespace DevBook.API.Features.BookStore.Books;

public record PatchBookCommandDto(
	string? Name,
	Guid? AuthorId,
	decimal? RetailPrice,
	decimal? Price,
	decimal? DiscountAmmount,
	string? Description,
	string? CoverImageUrl,
	IEnumerable<ProductCategory>? ProductCategories)
	: ICommand<OneOf<Success, NotFound>>;

public record PatchBookCommand(
	Guid Id,
	string? Name,
	Guid? AuthorId,
	decimal? RetailPrice,
	decimal? Price,
	decimal? DiscountAmmount,
	string? Description,
	string? CoverImageUrl,
	IEnumerable<ProductCategory>? ProductCategories)
	: ICommand<OneOf<Success, NotFound>>;

public sealed class PatchBookCommandValidator : AbstractValidator<PatchBookCommand>
{
	public PatchBookCommandValidator()
	{
		RuleFor(x => x.Id).NotEqual(Guid.Empty);
		When(x => x.AuthorId is not null, () => RuleFor(x => x.AuthorId).NotEqual(Guid.Empty));
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
			[nameof(Book.Name)] = command.Name ?? book.Name,
			[nameof(Book.AuthorId)] = command.AuthorId ?? book.AuthorId,
			[nameof(Book.RetailPrice)] = command.RetailPrice ?? book.RetailPrice,
			[nameof(Book.Price)] = command.Price ?? book.Price,
			[nameof(Book.DiscountAmmount)] = command.DiscountAmmount ?? book.DiscountAmmount,
			[nameof(Book.Description)] = command.Description ?? book.Description,
			[nameof(Book.CoverImageUrl)] = command.CoverImageUrl ?? book.CoverImageUrl,
			[nameof(Book.ProductCategories)] = command.ProductCategories ?? book.ProductCategories,
		};

		dbContext.Products.Entry(book).CurrentValues.SetValues(update);
		await dbContext.SaveChangesAsync(cancellationToken);
		return new Success();
	}
}
