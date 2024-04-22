namespace DevBook.API.Features.TimeTracking.Shared;

public sealed record ProjectDto : IMappebleTo<Project>
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
