namespace DevBook.API.Features.TimeTracking.Shared;

[Authorize]
public sealed record ProjectDto : IMappableTo<Project>
{
	[Required]
	public required Guid Id { get; init; }

	[Required]
	public required string Name { get; init; }
	public string? Details { get; init; }
	public int? HourlyRate { get; init; }
	public string? Currency { get; init; }
	public string? HexColor { get; init; }
}
