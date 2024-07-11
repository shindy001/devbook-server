using DevBook.API.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

public sealed class TestAuthInterceptor
{
	public Func<Task<AuthenticateResult>>? OnAuthenticate { get; set; }

	public void AuthenticateUser()
	{
		OnAuthenticate = () => Task.FromResult(GetUserAuthResult(DevBookUserRoles.User));
	}
	public void AuthenticateAdmin()
	{
		OnAuthenticate = () => Task.FromResult(GetUserAuthResult(DevBookUserRoles.Admin));
	}

	private AuthenticateResult GetUserAuthResult(string userRole)
	{
		var roleClaim = new Claim(ClaimTypes.Role, userRole);
		var claims = new[]
		{
			new Claim(ClaimTypes.Name, "Test user"),
			roleClaim
		};
		var identity = new ClaimsIdentity(claims, "Test");
		var principal = new ClaimsPrincipal(identity);
		var ticket = new AuthenticationTicket(principal, "TestScheme");
		return AuthenticateResult.Success(ticket);
	}
}
