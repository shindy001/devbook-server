namespace DevBook.API.Features.Sudoku;

internal interface ISudokuService
{
	public Task<BoardData> GetBoardData(CancellationToken cancellationToken);
}

/// <summary>
/// Using free sudoku api, see <see href="https://sudoku-api.vercel.app/" />
/// </summary>
/// <param name="httpClient"></param>
internal sealed class SudokuService(HttpClient httpClient) : ISudokuService
{
	public async Task<BoardData> GetBoardData(CancellationToken cancellationToken)
	{
		var response = await httpClient.GetAsync("dosuku", cancellationToken);
		response.EnsureSuccessStatusCode();
		var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

		JsonDocument data = JsonDocument.Parse(responseBody);
		JsonElement grids = data.RootElement
			.GetProperty("newboard")
			.GetProperty("grids")[0]; // Assuming there's only one grid, adjust accordingly if there are multiple

		var gridNumbers = grids.GetProperty("value").Deserialize<int[][]>();
		var solutionNumbers = grids.GetProperty("solution").Deserialize<int[][]>();

		return new BoardData
		{
			GridNumbers = gridNumbers ?? [],
			SolutionNumbers = solutionNumbers ?? []
		};
	}
}
