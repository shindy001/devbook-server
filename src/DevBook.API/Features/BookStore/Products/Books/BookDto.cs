namespace DevBook.API.Features.BookStore.Products.Books;

public sealed record BookDto : ProductDto
{
	[Required]
	public required Guid AuthorId { get; set; }
}
