namespace DevBook.API.Features.Sudoku;

public sealed record BoardData
{
	[Required]
	public ICollection<ICollection<int>> GridNumbers { get; init; }

	[Required]
	public ICollection<ICollection<int>> SolutionNumbers { get; init; }

	public BoardData(ICollection<ICollection<int>> gridNumbers, ICollection<ICollection<int>> solutionNumbers)
	{
		GridNumbers = gridNumbers;
		SolutionNumbers = solutionNumbers;
	}
}
