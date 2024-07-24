using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using System.Diagnostics;
using NotFound = Microsoft.AspNetCore.Http.HttpResults.NotFound;

namespace DevBook.API.Features.Account;

internal static class IdentityEndpoints
{
	private const string OperationIdPrefix = "Identity.";

	// Validate the email address using DataAnnotations like the UserValidator does when RequireUniqueEmail = true.
	private static readonly EmailAddressAttribute _emailAddressAttribute = new();

	public static RouteGroupBuilder MapIdentityEndpoints(this RouteGroupBuilder groupBuilder)
	{
		groupBuilder.MapPost("/register", Register)
			.WithName($"{OperationIdPrefix}Register")
			.Produces(StatusCodes.Status200OK)
			.AllowAnonymous();

		groupBuilder.MapPost("/login", Login)
			.WithName($"{OperationIdPrefix}Login")
			.Produces<AccessTokenResponse>()
			.Produces(StatusCodes.Status401Unauthorized)
			.AllowAnonymous();

		groupBuilder.MapGet("/info", GetInfo)
			.WithName($"{OperationIdPrefix}Info")
			.Produces<InfoResponse>();

		groupBuilder.MapPost("/refresh", Refresh)
			.WithName($"{OperationIdPrefix}Refresh")
			.Produces<AccessTokenResponse>()
			.AllowAnonymous();

		return groupBuilder;
	}

	// NOTE: We cannot inject UserManager<TUser> directly because the TUser generic parameter is currently unsupported by RDG.
	// https://github.com/dotnet/aspnetcore/issues/47338
	private static async Task<Results<Ok, ValidationProblem>> Register(
		[FromBody] RegisterRequest registration,
		HttpContext context,
		[FromServices] IServiceProvider sp)
	{
		var userManager = sp.GetRequiredService<UserManager<DevBookUser>>();
		var userStore = sp.GetRequiredService<IUserStore<DevBookUser>>();
		var emailStore = (IUserEmailStore<DevBookUser>)userStore;
		var email = registration.Email;

		if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
		{
			return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email)));
		}

		var user = new DevBookUser();
		await userStore.SetUserNameAsync(user, email, CancellationToken.None);
		await emailStore.SetEmailAsync(user, email, CancellationToken.None);
		var result = await userManager.CreateAsync(user, registration.Password);

		if (!result.Succeeded)
		{
			return CreateValidationProblem(result);
		}

		return TypedResults.Ok();
	}

	private static async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> Login(
		[FromBody] LoginRequest login,
		[FromServices] IServiceProvider sp)
	{
		var signInManager = sp.GetRequiredService<SignInManager<DevBookUser>>();
		signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;

		var result = await signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent: false, lockoutOnFailure: false);
		return result.Succeeded
			? TypedResults.Empty
			: TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
	}

	public static async Task<Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>> Refresh(
		[FromBody] RefreshRequest refreshRequest,
		[FromServices] IServiceProvider sp)
	{
		var timeProvider = sp.GetRequiredService<TimeProvider>();
		var bearerTokenOptions = sp.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>();
		var signInManager = sp.GetRequiredService<SignInManager<DevBookUser>>();
		var refreshTokenProtector = bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
		var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

		// Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
		if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
			timeProvider.GetUtcNow() >= expiresUtc ||
			await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not DevBookUser user)

		{
			return TypedResults.Challenge();
		}

		var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
		return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
	}

	private static async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> GetInfo(
		ClaimsPrincipal claimsPrincipal,
		[FromServices] IServiceProvider sp)
	{
		var userManager = sp.GetRequiredService<UserManager<DevBookUser>>();
		if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
		{
			return TypedResults.NotFound();
		}

		return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
	}

	private static ValidationProblem CreateValidationProblem(IdentityResult result)
	{
		// We expect a single error code and description in the normal case.
		// This could be golfed with GroupBy and ToDictionary, but perf! :P
		Debug.Assert(!result.Succeeded);
		var errorDictionary = new Dictionary<string, string[]>(1);

		foreach (var error in result.Errors)
		{
			string[] newDescriptions;

			if (errorDictionary.TryGetValue(error.Code, out var descriptions))
			{
				newDescriptions = new string[descriptions.Length + 1];
				Array.Copy(descriptions, newDescriptions, descriptions.Length);
				newDescriptions[descriptions.Length] = error.Description;
			}
			else
			{
				newDescriptions = [error.Description];
			}

			errorDictionary[error.Code] = newDescriptions;
		}

		return TypedResults.ValidationProblem(errorDictionary);
	}

	private static async Task<InfoResponse> CreateInfoResponseAsync<TUser>(TUser user, UserManager<TUser> userManager)
		where TUser : class
	{
		return new()
		{
			Email = await userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
			Roles = await userManager.GetRolesAsync(user),
		};
	}
}
