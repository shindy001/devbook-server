﻿using System.Reflection;

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

	internal static IServiceCollection RegisterAuthentication(this IServiceCollection services)
	{
		services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme, opt => opt.BearerTokenExpiration = TimeSpan.FromMinutes(30));
		services.AddAuthorizationBuilder();
		services.AddIdentityCore<DevBookUser>()
			.AddEntityFrameworkStores<DevBookDbContext>()
			.AddApiEndpoints();

		return services;
	}

	internal static IServiceCollection RegisterDB(this IServiceCollection services)
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
