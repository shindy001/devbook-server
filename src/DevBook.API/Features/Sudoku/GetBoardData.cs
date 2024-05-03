using HotChocolate.Authorization;

namespace DevBook.API.Features.Sudoku;

internal sealed record GetBoardDataQueryInput : IQuery<BoardData>;

internal sealed class GetBoardDataQueryHandler(ISudokuService sudokuService) : IQueryHandler<GetBoardDataQueryInput, BoardData>
{
	public async Task<BoardData> Handle(GetBoardDataQueryInput request, CancellationToken cancellationToken)
	{
		return await sudokuService.GetBoardData(cancellationToken);
	}
}

[QueryType]
internal sealed class GetBoardDataQuery
{
	[AllowAnonymous]
	public async Task<BoardData> GetBoardData(IExecutor executor, CancellationToken cancellationToken)
	{
		return await executor.ExecuteQuery(new GetBoardDataQueryInput(), cancellationToken);
	}
}
