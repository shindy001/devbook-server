namespace DevBook.API.Identity;

public static class HttpContextAccessorExtensions
{
	internal static IServiceCollection RegisterAuthentication(this IServiceCollection services, int tokenTTLinMinutes = 30)
	{
		services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme, opt => opt.BearerTokenExpiration = TimeSpan.FromMinutes(tokenTTLinMinutes));
		services.AddAuthorizationBuilder();
		services.AddIdentityCore<DevBookUser>()
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
	public static Guid GetUserId(this IHttpContextAccessor accessor)
	{
		var user = accessor.HttpContext?.User;

		if (user is null || user.Identity is null || user.Identity?.IsAuthenticated == false)
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
