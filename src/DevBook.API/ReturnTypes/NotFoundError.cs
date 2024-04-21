namespace DevBook.API.ReturnTypes;

public sealed record NotFoundError : IGraphQLError
{
	public required Guid Id { get; init; }

	public string Message => "Could not find item with specified id.";
}
