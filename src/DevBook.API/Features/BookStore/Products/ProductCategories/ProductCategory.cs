namespace DevBook.API.Features.BookStore.Products.ProductCategories;

public sealed record ProductCategory()
	: Entity(Guid.NewGuid())
{
	public required string Name { get; init; }
}
