using Refit;
namespace DevBook.API.IntegrationTests.Api;

public interface ISudokuApi
{
	[Get("/sudoku")]
	Task<BoardDataResponse> GetBoardData();
}

public sealed record BoardDataResponse
{
	public ICollection<ICollection<int>> GridNumbers { get; init; } = [];
	public ICollection<ICollection<int>> SolutionNumbers { get; init; } = [];
}
