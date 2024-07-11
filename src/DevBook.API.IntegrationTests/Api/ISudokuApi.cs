using DevBook.API.Features.Sudoku;

namespace DevBook.API.IntegrationTests.Api;

public interface ISudokuApi
{
	[Get("/sudoku")]
	Task<BoardDataDto> GetBoardData();
}
