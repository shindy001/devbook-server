namespace DevBook.API.Features.Sudoku;

internal sealed record GetBoardDataQuery : IQuery<BoardData>;

internal sealed class GetBoardDataQueryHandler(ISudokuService sudokuService) : IQueryHandler<GetBoardDataQuery, BoardData>
{
	public async Task<BoardData> Handle(GetBoardDataQuery request, CancellationToken cancellationToken)
	{
		return await sudokuService.GetBoardData(cancellationToken);
	}
}

[QueryType]
internal sealed class BoardDataQuery
{
	public async Task<BoardData> GetBoardData(IExecutor executor, CancellationToken cancellationToken)
	{
		return await executor.ExecuteQuery(new GetBoardDataQuery(), cancellationToken);
	}
}
