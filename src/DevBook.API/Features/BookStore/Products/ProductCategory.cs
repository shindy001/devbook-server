namespace DevBook.API.Features.BookStore.Products;

public sealed record ProductCategory()
	: Entity(Guid.NewGuid())
{
	public required string Name { get; init; }
}
