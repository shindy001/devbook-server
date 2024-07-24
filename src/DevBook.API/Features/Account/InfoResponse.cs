namespace DevBook.API.Features.Account;

public sealed record InfoResponse
{
	[Required]
	public required string Email { get; init; }

	[Required]
	public required IList<string> Roles { get; init; } = [];
}
