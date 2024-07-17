using AutoFixture;
using DevBook.API.Features.BookStore.Authors;
using DevBook.API.Features.BookStore.Products.Books;
using System.Net;

namespace DevBook.API.IntegrationTests.Features.Products;

public class BookApiTests : IntegrationTestsBase
{
	private readonly IBookStoreApi _bookStoreApi;
	private readonly BookStoreApiDriver _bookStoreDriver;
	private readonly Fixture _fixture = new();

	public BookApiTests(ITestOutputHelper outputHelper) : base(outputHelper)
	{
		_bookStoreApi = GetClient<IBookStoreApi>();
		_bookStoreDriver = new BookStoreApiDriver(_bookStoreApi, TestAuthInterceptor);
	}

	#region CreateBook

	[Fact]
	public async Task CreateBook_should_succeed_for_Admin_user()
	{
		// Given
		AuthenticateAdmin();
		var givenAuthorId = await _bookStoreDriver.SeedAuthor(_fixture.Create<CreateAuthorCommand>());
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.AuthorId, givenAuthorId)
			.With(x => x.ProductCategoryIds, [])
			.Create();

		// When
		var response = await _bookStoreApi.CreateBook(givenCreateBookCommand);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Created);

		var newItemId = response.Headers.Location?.Segments[^1];
		newItemId.Should().NotBeNullOrEmpty();
		Guid.TryParse(newItemId, out _).Should().BeTrue();
	}

	[Fact]
	public async Task CreateBook_should_return_401_unauthorized_when_user_is_not_authenticated()
	{
		// Given
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.AuthorId, Guid.NewGuid())
			.With(x => x.ProductCategoryIds, [])
			.Create();

		// When
		var response = await _bookStoreApi.CreateBook(givenCreateBookCommand);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}

	[Fact]
	public async Task CreateBook_should_return_403_forbidden_when_standard_user_is_authenticated()
	{
		// Given
		AuthenticateUser();
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.AuthorId, Guid.NewGuid())
			.With(x => x.ProductCategoryIds, [])
			.Create();

		// When
		var response = await _bookStoreApi.CreateBook(givenCreateBookCommand);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
	}

	[Fact]
	public async Task CreateBook_should_return_400_badRequest_when_create_command_has_empty_name()
	{
		// Given
		AuthenticateAdmin();
		var givenAuthorId = await _bookStoreDriver.SeedAuthor(_fixture.Create<CreateAuthorCommand>());
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.Name, string.Empty)
			.With(x => x.AuthorId, givenAuthorId)
			.With(x => x.ProductCategoryIds, [])
			.Create();

		// When
		var response = await _bookStoreApi.CreateBook(givenCreateBookCommand);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	[Fact]
	public async Task CreateBook_should_return_400_badRequest_when_createBook_command_contains_AuthorId_that_does_not_exist()
	{
		// Given
		AuthenticateAdmin();
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.AuthorId, Guid.NewGuid())
			.With(x => x.ProductCategoryIds, [])
			.Create();

		// When
		var response = await _bookStoreApi.CreateBook(givenCreateBookCommand);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	[Fact]
	public async Task CreateBook_should_return_400_badRequest_when_createBook_command_contains_ProductCategoryId_that_does_not_exist()
	{
		// Given
		AuthenticateAdmin();
		var givenAuthorId = await _bookStoreDriver.SeedAuthor(_fixture.Create<CreateAuthorCommand>());
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.AuthorId, givenAuthorId)
			.With(x => x.ProductCategoryIds, [Guid.NewGuid()])
			.Create();

		// When
		var response = await _bookStoreApi.CreateBook(givenCreateBookCommand);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	#endregion
}
