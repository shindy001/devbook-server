namespace DevBook.API.Features.BookStore.Authors;

public sealed record Author()
	: Entity(Guid.NewGuid())
{
	public required string Name { get; init; }
	public string? Description { get; init; }
}
