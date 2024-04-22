namespace DevBook.API.Features.Sudoku;

public sealed record BoardData
{
	[Required]
	public required ICollection<ICollection<int>> GridNumbers { get; init; } = [];

	[Required]
	public required ICollection<ICollection<int>> SolutionNumbers { get; init; } = [];
}
