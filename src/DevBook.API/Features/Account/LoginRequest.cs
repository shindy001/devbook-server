namespace DevBook.API.Features.Account;

public sealed record LoginRequest
{
	[Required]
	public required string Email { get; init; }

	[Required]
	public required string Password { get; init; }
}
