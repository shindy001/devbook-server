namespace DevBook.API.IntegrationTests;

public class IntegrationTestsBase : IDisposable
{
	public TestAuthInterceptor TestAuthInterceptor { get; private set; } = null!;

	private readonly DevBookApiTestFixture<Program> _devBookApiTestFixture;
	private readonly IDisposable? _testContext;
	private HttpClient? _httpClient;

	public IntegrationTestsBase(ITestOutputHelper outputHelper)
	{
		_devBookApiTestFixture = new DevBookApiTestFixture<Program>();
		_testContext = _devBookApiTestFixture.GetTestContext(outputHelper);
	}

	public T GetClient<T>()
	{
		_httpClient = _devBookApiTestFixture.CreateClient();
		TestAuthInterceptor = _devBookApiTestFixture.GetTestAuthInterceptor();
		return RestService.For<T>(_httpClient);
	}

	public void ReplaceService<TService>(object instance)
	{
		if (_httpClient is not null)
		{
			throw new InvalidOperationException("Cannot replace service after Client creation. Replace first and then create client.");
		}

		_devBookApiTestFixture.ReplaceService<TService>(instance);
	}

	public void AuthenticateUser()
	{
		if (_httpClient is null)
		{
			throw new InvalidOperationException("Cannot Authenticate user before Client creation. Create client and then authenticate user.");
		}

		TestAuthInterceptor.AuthenticateUser();
	}

	public void AuthenticateAdmin()
	{
		if (_httpClient is null)
		{
			throw new InvalidOperationException("Cannot Authenticate user before Client creation. Create client and then authenticate user.");
		}

		TestAuthInterceptor.AuthenticateAdmin();
	}

	public void Dispose()
	{
		_testContext?.Dispose();
		_devBookApiTestFixture?.Dispose();
	}
}
