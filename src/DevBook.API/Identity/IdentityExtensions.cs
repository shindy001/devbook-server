﻿namespace DevBook.API.Identity;

public static class IdentityExtensions
{
	/// <summary>
	/// Registers authentication and identity stores, uses <see cref="DevBookDbContext"/> for stores.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="tokenTTLinMinutes"></param>
	/// <param name="requireConfirmedAccountOnSignIn"></param>
	/// <returns></returns>
	internal static IServiceCollection RegisterAuth(this IServiceCollection services, int tokenTTLinMinutes = 30, bool requireConfirmedAccountOnSignIn = true)
	{
		services.AddAuthentication().AddBearerToken(
			IdentityConstants.BearerScheme,
			opt => opt.BearerTokenExpiration = TimeSpan.FromMinutes(tokenTTLinMinutes));

		services.AddAuthorizationBuilder()
			.AddPolicy(DevBookAccessPolicies.RequireAdmin, policy => policy.RequireRole(DevBookUserRoles.Admin));
		services.AddIdentityCore<DevBookUser>(options => options.SignIn.RequireConfirmedAccount = requireConfirmedAccountOnSignIn)
			.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<DevBookDbContext>()
			.AddApiEndpoints();

		return services;
	}

	/// <summary>
	/// Gets userId from HttpContext in HttpContextAccessor
	/// </summary>
	/// <param name="accessor"></param>
	/// <returns>UserId as Guid</returns>
	/// <exception cref="UnauthorizedAccessException">When user is null or not authenticated</exception>
	/// <exception cref="InvalidOperationException">When NameIdentifier claim is missing</exception>
	internal static Guid GetUserId(this IHttpContextAccessor accessor)
	{
		var user = accessor.HttpContext?.User;

		if (user?.Identity is null || user.Identity?.IsAuthenticated == false)
		{
			throw new UnauthorizedAccessException();
		}

		var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrEmpty(userId))
		{
			throw new InvalidOperationException("Missing NameIdentifier claim.");
		}

		return Guid.Parse(userId);
	}
}
