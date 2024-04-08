
namespace DevBook.API.Features.Sudoku;

internal sealed class SudokuModule : IFeatureModule
{
	// move api address to appsettings if needed
	private const string SudokuApiAddress = "https://sudoku-api.vercel.app/api/";

	public IServiceCollection RegisterModule(IServiceCollection services)
	{
		services.AddHttpClient<ISudokuService, SudokuService>(
			opt => opt.BaseAddress = new Uri(SudokuApiAddress));
		return services;
	}

	public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpointsBuilder)
	{
		endpointsBuilder
			.MapGroup("/sudoku")
			.MapSudokuEndpoints()
			.WithTags($"{nameof(SudokuModule)}_{nameof(SudokuEndpoints)}");

		return endpointsBuilder;
	}
}
