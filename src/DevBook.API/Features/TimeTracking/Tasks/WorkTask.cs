namespace DevBook.API.Features.TimeTracking.Tasks;

public sealed record WorkTask(
	Guid? ProjectId,
	string? Description,
	string? Details,
	DateTimeOffset Date,
	TimeOnly Start,
	TimeOnly? End)
	: Entity(Guid.NewGuid());
