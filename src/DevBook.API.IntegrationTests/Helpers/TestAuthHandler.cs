using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

internal class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
	private readonly TestAuthInterceptor _testAuthInterceptor;

	public TestAuthHandler(
		IOptionsMonitor<AuthenticationSchemeOptions> options,
		ILoggerFactory logger,
		UrlEncoder encoder,
		TestAuthInterceptor testAuthInterceptor)
		: base(options, logger, encoder)
	{
		_testAuthInterceptor = testAuthInterceptor;
	}

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		if (_testAuthInterceptor.OnAuthenticate is not null)
		{
			return _testAuthInterceptor.OnAuthenticate();
		}

		return Task.FromResult(AuthenticateResult.Fail($"Authentication failed. Handler: {nameof(TestAuthHandler)}"));
	}
}
