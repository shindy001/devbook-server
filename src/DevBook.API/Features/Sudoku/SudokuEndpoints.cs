namespace DevBook.API.Features.Sudoku;

internal static class SudokuEndpoints
{
	private const string OperationIdPrefix = "Sudoku.";

	public static RouteGroupBuilder MapSudokuEndpoints(this RouteGroupBuilder groupBuilder)
	{
		groupBuilder.MapGet("/", GetBoard)
			.WithName($"{OperationIdPrefix}Board")
			.Produces<BoardData>();

		return groupBuilder;
	}

	private static async Task<IResult> GetBoard(IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(new GetBoardDataQueryInput(), cancellationToken);
		return TypedResults.Ok(result);
	}
}
