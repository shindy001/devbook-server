namespace DevBook.API.Features.Sudoku;

internal sealed record GetBoardDataQueryInput : IQuery<BoardDataDto>;

internal sealed class GetBoardDataQueryHandler(ISudokuService sudokuService) : IQueryHandler<GetBoardDataQueryInput, BoardDataDto>
{
	public async Task<BoardDataDto> Handle(GetBoardDataQueryInput request, CancellationToken cancellationToken)
	{
		return await sudokuService.GetBoardData(cancellationToken);
	}
}

[QueryType]
internal sealed class GetBoardDataQuery
{
	[AllowAnonymous]
	public async Task<BoardDataDto> GetBoardData(IExecutor executor, CancellationToken cancellationToken)
	{
		return await executor.ExecuteQuery(new GetBoardDataQueryInput(), cancellationToken);
	}
}
