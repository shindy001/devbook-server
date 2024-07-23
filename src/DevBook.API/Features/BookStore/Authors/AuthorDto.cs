namespace DevBook.API.Features.BookStore.Authors;

public sealed record AuthorDto
{
	[Required]
	public required Guid Id { get; init; }

	[Required]
	public required string Name { get; init; }
	public string? Description { get; init; }
}

public static class AuthorMappings
{
	public static AuthorDto ToDto(this Author author)
	{
		return new AuthorDto
		{
			Id = author.Id,
			Name = author.Name,
			Description = author.Description,
		};
	}
}
