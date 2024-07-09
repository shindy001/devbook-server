namespace DevBook.API.Features.BookStore.Shared;

public sealed record ProductCategory()
	: Entity(Guid.NewGuid())
{
	public required string Name { get; init; }
}
