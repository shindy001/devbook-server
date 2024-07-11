using DevBook.API.Features.Sudoku;

namespace DevBook.API.IntegrationTests.Features.Sudoku;

public class SudokuApiTests : IntegrationTestsBase
{
	private readonly ISudokuService _sudokuService = Substitute.For<ISudokuService>();
	private readonly ISudokuApi _sudokuApi;
	private readonly BoardDataDto _givenBoardData = new()
	{
		GridNumbers =
		[
			[7, 0, 6, 0, 0, 0, 0, 0, 0],
			[0, 0, 0, 0, 2, 7, 0, 0, 0],
			[0, 0, 9, 0, 0, 0, 7, 0, 8],
			[0, 8, 0, 2, 0, 9, 0, 0, 3],
			[0, 0, 5, 0, 0, 0, 9, 1, 2],
			[6, 9, 0, 3, 0, 5, 8, 7, 4],
			[0, 2, 0, 0, 0, 4, 6, 9, 0],
			[8, 0, 0, 0, 9, 0, 4, 0, 7],
			[0, 0, 0, 1, 0, 3, 0, 0, 5]
		],
		SolutionNumbers =
		[
			[7, 4, 6, 8, 5, 1, 3, 2, 9],
			[3, 5, 8, 9, 2, 7, 1, 4, 6],
			[2, 1, 9, 4, 3, 6, 7, 5, 8],
			[1, 8, 7, 2, 4, 9, 5, 6, 3],
			[4, 3, 5, 6, 7, 8, 9, 1, 2],
			[6, 9, 2, 3, 1, 5, 8, 7, 4],
			[5, 2, 3, 7, 8, 4, 6, 9, 1],
			[8, 6, 1, 5, 9, 2, 4, 3, 7],
			[9, 7, 4, 1, 6, 3, 2, 8, 5]
		],
	};

	public SudokuApiTests(ITestOutputHelper outputHelper) : base(outputHelper)
	{
		this.ReplaceService<ISudokuService>(_sudokuService);
		_sudokuApi = this.GetClient<ISudokuApi>();
	}

	[Fact]
	public async Task GetBoardData_should_return_board_data()
	{
		// Given
		var givenBoardData = _givenBoardData;
		_sudokuService.GetBoardData(Arg.Any<CancellationToken>()).Returns(givenBoardData);

		// When
		var response = await _sudokuApi.GetBoardData();

		// Then
		response.Should().NotBeNull();
		response.GridNumbers.Should().NotBeEmpty();
		response.SolutionNumbers.Should().NotBeEmpty();

		response.GridNumbers.Should().BeEquivalentTo(givenBoardData.GridNumbers);
		response.SolutionNumbers.Should().BeEquivalentTo(givenBoardData.SolutionNumbers);
	}
}
