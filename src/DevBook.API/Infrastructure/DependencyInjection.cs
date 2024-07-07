using System.Reflection;

namespace DevBook.API.Infrastructure;

internal static class DependencyInjection
{
	private readonly static Assembly Assembly = typeof(Program).Assembly;

	internal static IServiceCollection RegisterRequestPipelines(this IServiceCollection services)
	{
		services.AddSingleton(TimeProvider.System);
		services.AddHttpContextAccessor();

		services.AddCommandsAndQueriesExecutor(Assembly);

		// Register FluentValidation validators
		services.AddValidatorsFromAssembly(Assembly);
		services.AddPipelineBehavior(typeof(ValidationPipelineBehavior<,>));

		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddPipelineBehavior(typeof(UnitOfWorkCommandPipelineBehavior<,>));
		services.AddPipelineBehavior(typeof(UnitOfWorkQueryPipelineBehavior<,>));

		return services;
	}

	internal static IServiceCollection RegisterDevBookDbContext(this IServiceCollection services)
	{
		services.AddDbContextPool<DevBookDbContext>(
			opt => opt.UseSqlite(
				GetSqliteConnectionString(),
				opt => opt.MigrationsAssembly(Assembly.GetName().Name)));

		return services;
	}

	internal static IApplicationBuilder InitializeDb(this IApplicationBuilder builder, bool applyMigrations = false)
	{
		using var scope = builder.ApplicationServices.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<DevBookDbContext>();
		db.Database.EnsureCreated();

		if (applyMigrations)
		{
			db.Database.Migrate();
		}

		return builder;
	}

	public static async Task<IApplicationBuilder> SeedRoles(this IApplicationBuilder builder, params string[] roles)
	{
		using var scope = builder.ApplicationServices.CreateScope();
		var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

		foreach (var role in roles)
		{
			if (!await roleManager.RoleExistsAsync(role))
			{
				await roleManager.CreateAsync(new IdentityRole(role));
			}
		}

		return builder;
	}

	public static async Task<IApplicationBuilder> SeedUsers(this IApplicationBuilder builder, params UserDbSeed[] userDbSeeds)
	{
		using var scope = builder.ApplicationServices.CreateScope();
		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<DevBookUser>>();

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

		return builder;
	}

	private static string GetSqliteConnectionString()
	{
		var dbPath = System.IO.Path.Combine(AppContext.BaseDirectory, "data", $"DevBook.db");

		if (!Directory.Exists(dbPath))
		{
			Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dbPath)!);
		}

		return $"Data Source={dbPath}";
	}
}
