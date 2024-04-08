namespace DevBook.API.Features.TimeTracking.Shared;

public sealed record ProjectDto
{
	[Required]
	public Guid Id { get; init; }

	[Required]
	public string Name { get; init; }
	public string? Details { get; init; }
	public int? HourlyRate { get; init; }
	public string? Currency { get; init; }
	public string? HexColor { get; init; }

	public ProjectDto(Guid id, string name, string? details, int? hourlyRate, string? currency, string? hexColor)
	{
		Id = id;
		Name = name;
		Details = details;
		HourlyRate = hourlyRate;
		Currency = currency;
		HexColor = hexColor;
	}
}
