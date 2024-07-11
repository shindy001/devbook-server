namespace DevBook.API.IntegrationTests;

public class IntegrationTestsBase : IDisposable
{
	private readonly DevBookApiTestFixture<Program> _fixture;
	private TestAuthInterceptor? _testAuthInterceptor;
	private readonly IDisposable? _testContext;
	private HttpClient? _httpClient;

	public IntegrationTestsBase(ITestOutputHelper outputHelper)
	{
		_fixture = new DevBookApiTestFixture<Program>();
		_testContext = _fixture.GetTestContext(outputHelper);
	}

	public T GetClient<T>()
	{
		_httpClient = _fixture.CreateClient();
		_testAuthInterceptor = _fixture.GetTestAuthInterceptor();
		return RestService.For<T>(_httpClient);
	}

	public void ReplaceService<TService>(object instance)
	{
		if (_httpClient is not null)
		{
			throw new InvalidOperationException("Cannot replace service after Client creation. Replace first and then create client.");
		}

		_fixture.ReplaceService<TService>(instance);
	}

	public void AuthenticatedUser()
	{
		if (_httpClient is null)
		{
			throw new InvalidOperationException("Cannot Authenticate user before Client creation. Create client and then authenticate user.");
		}

		_testAuthInterceptor?.AuthenticateUser();
	}

	public void AuthenticatedAdmin()
	{
		if (_httpClient is null)
		{
			throw new InvalidOperationException("Cannot Authenticate user before Client creation. Create client and then authenticate user.");
		}

		_testAuthInterceptor?.AuthenticateAdmin();
	}

	public void Dispose()
	{
		_testContext?.Dispose();
		_fixture?.Dispose();
	}
}
