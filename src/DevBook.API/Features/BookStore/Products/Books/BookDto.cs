namespace DevBook.API.Features.BookStore.Products.Books;

public sealed record BookDto : ProductDto
{
	public string? Author { get; init; }
}
