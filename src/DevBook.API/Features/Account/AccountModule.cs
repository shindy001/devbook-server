

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

	public async Task InitializeModule(AsyncServiceScope serviceScope)
	{
		var configuration = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
		var defaultUsers = configuration.GetSection("DefaultUsers").Get<UserDbSeed[]>();

		// Seed roles to DB if not defined
		await SeedRoles(
			serviceScope,
			DevBookUserRoles.Admin,
			DevBookUserRoles.User);

		// Seed default users from appsettings to DB
		if (defaultUsers is not null && defaultUsers.Length != 0)
		{
			await SeedUsers(serviceScope, defaultUsers);
		}
	}

	private static async Task SeedRoles(AsyncServiceScope serviceScope, params string[] roles)
	{
		var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

		foreach (var role in roles)
		{
			if (!await roleManager.RoleExistsAsync(role))
			{
				await roleManager.CreateAsync(new IdentityRole(role));
			}
		}
	}

	private static async Task SeedUsers(AsyncServiceScope serviceScope, params UserDbSeed[] userDbSeeds)
	{
		var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<DevBookUser>>();

		foreach (var userSeed in userDbSeeds)
		{
			if (await userManager.FindByEmailAsync(userSeed.Email) is null)
			{
				var user = new DevBookUser
				{
					Email = userSeed.Email,
					UserName = userSeed.Email,
					EmailConfirmed = userSeed.EmailConfirmed,
				};

				await userManager.CreateAsync(user, userSeed.Password);
				await userManager.AddToRoleAsync(user, userSeed.UserRole);
			}
		}
	}
}
