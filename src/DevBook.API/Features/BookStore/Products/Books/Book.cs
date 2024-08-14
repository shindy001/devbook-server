namespace DevBook.API.Features.BookStore.Products.Books;

public sealed record Book : Product
{
	public string? Author { get; init; }
}
