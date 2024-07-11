using DevBook.API.Features.BookStore.Shared;

namespace DevBook.API.Features.BookStore.Products.Books;

public sealed record Book : Product
{
	public required Guid AuthorId { get; set; }
}
