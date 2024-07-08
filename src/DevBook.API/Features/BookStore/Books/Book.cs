namespace DevBook.API.Features.BookStore.Books;

public sealed record Book()
	: Entity(Guid.NewGuid())
{
	public required string Name { get; init; }
	public required Guid AuthorId { get; set; }
	public string? Description { get; init; }
	public string? CoverImageUrl { get; init; }
	public string[] BookCategories { get; set; } = [];

	public required decimal RetailPrice { get; init; }
	public required decimal Price { get; init; }
	public required decimal DiscountAmmount { get; init; }
}
