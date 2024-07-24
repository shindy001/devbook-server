
namespace DevBook.API.Features.Account;

internal sealed class AccountModule : IFeatureModule
{
	public IServiceCollection RegisterModule(IServiceCollection services)
	{
		return services;
	}

	public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpointsBuilder)
	{
		endpointsBuilder
			.MapGroup("/identity")
			.MapIdentityEndpoints()
			.WithTags($"{nameof(AccountModule)}_{nameof(IdentityEndpoints)}")
			.RequireAuthorization();

		return endpointsBuilder;
	}
}
