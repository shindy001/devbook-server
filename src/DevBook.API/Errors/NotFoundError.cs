namespace DevBook.API.Errors;

internal sealed record NotFoundError : IGraphQLError
{
	public required Guid Id { get; init; }

	public string Message => "Could not find item with specified id.";
}
