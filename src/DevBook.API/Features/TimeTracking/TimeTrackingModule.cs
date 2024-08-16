using DevBook.API.Features.TimeTracking.Projects;
using DevBook.API.Features.TimeTracking.Tasks;

namespace DevBook.API.Features.TimeTracking;

internal sealed class TimeTrackingModule : IFeatureModule
{
	public IServiceCollection RegisterModule(IServiceCollection services)
	{
		return services;
	}

	public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpointsBuilder)
	{
		endpointsBuilder
			.MapGroup("/projects")
			.MapProjectEndpoints()
			.WithTags($"{nameof(TimeTrackingModule)}_{nameof(ProjectEndpoints)}")
			.RequireAuthorization();

		endpointsBuilder
			.MapGroup("/tasks")
			.MapWorkTaskEndpoints()
			.WithTags($"{nameof(TimeTrackingModule)}_{nameof(WorkTaskEndpoints)}")
			.RequireAuthorization();

		return endpointsBuilder;
	}

	public Task InitializeModule(AsyncServiceScope serviceScope)
	{
		return Task.CompletedTask;
	}
}
