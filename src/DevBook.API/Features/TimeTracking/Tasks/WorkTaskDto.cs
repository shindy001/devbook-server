namespace DevBook.API.Features.TimeTracking.Tasks;

public sealed record WorkTaskDto : IMappebleTo<WorkTask>
{
	[Required]
	public Guid Id { get; init; }
	public ProjectDto? Project { get; init; }
	public string? Description { get; init; }
	public string? Details { get; init; }

	[Required]
	public DateTimeOffset Date { get; init; }

	[Required]
	public TimeOnly Start { get; init; }
	public TimeOnly? End { get; init; }
}
