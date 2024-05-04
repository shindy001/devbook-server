namespace DevBook.API.Features.Sudoku;

[Authorize]
public sealed record BoardDataDto
{
	[Required]
	public required ICollection<ICollection<int>> GridNumbers { get; init; } = [];

	[Required]
	public required ICollection<ICollection<int>> SolutionNumbers { get; init; } = [];
}
