namespace DevBook.API.Features.TimeTracking.Shared;

public sealed record Project()
	: Entity(Guid.NewGuid())
{
	public required string Name { get; init; }
	public string? Details { get; init; }
	public int? HourlyRate { get; init; }
	public string? Currency { get; init; }
	public string? HexColor { get; init; }
}

public static class ProjectExtensions
{
	public static ProjectDto ToDto(this Project project) =>
		new()
		{
			Id = project.Id,
			Name = project.Name,
			Details = project.Details,
			HourlyRate = project.HourlyRate,
			Currency = project.Currency,
			HexColor = project.HexColor
		};
}
