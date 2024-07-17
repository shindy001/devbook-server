using AutoFixture;
using DevBook.API.Features.BookStore.Authors;
using DevBook.API.Features.BookStore.Products;
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
	public async Task CreateBook_should_create_book_when_user_is_Admin()
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
		var actualBook = (await _bookStoreApi.GetProducts<Book>()).Single();

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Created);
		var actualBookGuid = GetGuidFromResponseLocation(response);
		actualBookGuid.Should().NotBeNull();
		actualBook.Should().BeEquivalentTo(
			new Book
			{
				Id = actualBookGuid!.Value,
				Name = givenCreateBookCommand.Name,
				ProductType = ProductType.Book,
				AuthorId = givenAuthorId,
				RetailPrice = givenCreateBookCommand.RetailPrice,
				Price = givenCreateBookCommand.Price,
				DiscountAmmount = givenCreateBookCommand.DiscountAmmount,
				Description = givenCreateBookCommand.Description,
				CoverImageUrl = givenCreateBookCommand.CoverImageUrl,
				ProductCategoryIds = givenCreateBookCommand.ProductCategoryIds ?? []
			});
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

	#region UpdateBook

	[Fact]
	public async Task UpdateBook_should_update_book_when_user_is_Admin()
	{
		// Given
		AuthenticateAdmin();
		var givenAuthorId1 = await _bookStoreDriver.SeedAuthor(_fixture.Create<CreateAuthorCommand>());
		var givenAuthorId2 = await _bookStoreDriver.SeedAuthor(_fixture.Create<CreateAuthorCommand>());
		var givenCreateBookSeedCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.AuthorId, givenAuthorId1)
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenBookId = await _bookStoreDriver.SeedBook(givenCreateBookSeedCommand);
		var givenUpdateBookCommand = _fixture
			.Build<UpdateBookCommandDto>()
			.With(x => x.AuthorId, givenAuthorId2)
			.With(x => x.ProductCategoryIds, [])
			.Create();

		// When
		var response = await _bookStoreApi.UpdateBook(givenBookId, givenUpdateBookCommand);
		var actualBook = await _bookStoreApi.GetProductById<Book>(givenBookId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		actualBook.Should().BeEquivalentTo(
			new Book
			{
				Id = givenBookId,
				Name = givenUpdateBookCommand.Name,
				ProductType = ProductType.Book,
				AuthorId = givenAuthorId2,
				RetailPrice = givenUpdateBookCommand.RetailPrice,
				Price = givenUpdateBookCommand.Price,
				DiscountAmmount = givenUpdateBookCommand.DiscountAmmount,
				Description = givenUpdateBookCommand.Description,
				CoverImageUrl = givenUpdateBookCommand.CoverImageUrl,
				ProductCategoryIds = givenUpdateBookCommand.ProductCategoryIds ?? []
			});
	}

	[Fact]
	public async Task UpdateBook_should_return_401_unauthorized_when_user_is_not_authorized()
	{
		// Given
		// When
		var response = await _bookStoreApi.UpdateBook(Guid.NewGuid(), _fixture.Create<UpdateBookCommandDto>());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}

	[Fact]
	public async Task UpdateBook_should_return_403_forbidden_when_user_is_standard_user()
	{
		// Given
		AuthenticateUser();

		// When
		var response = await _bookStoreApi.UpdateBook(Guid.NewGuid(), _fixture.Create<UpdateBookCommandDto>());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
	}

	#endregion

	#region PatchBook

	[Fact]
	public async Task PatchBook_should_update_book_when_user_is_Admin()
	{
		// Given
		AuthenticateAdmin();
		var givenAuthorId1 = await _bookStoreDriver.SeedAuthor(_fixture.Create<CreateAuthorCommand>());
		var givenAuthorId2 = await _bookStoreDriver.SeedAuthor(_fixture.Create<CreateAuthorCommand>());
		var givenCreateBookSeedCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.AuthorId, givenAuthorId1)
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenBookId = await _bookStoreDriver.SeedBook(givenCreateBookSeedCommand);
		var givenPatchBookCommand = _fixture
			.Build<PatchBookCommandDto>()
			.With(x => x.AuthorId, givenAuthorId2)
			.With(x => x.ProductCategoryIds, [])
			.Create();

		// When
		var response = await _bookStoreApi.PatchBook(givenBookId, givenPatchBookCommand);
		var actualBook = await _bookStoreApi.GetProductById<Book>(givenBookId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		actualBook.Should().BeEquivalentTo(
			new Book
			{
				Id = givenBookId,
				Name = givenPatchBookCommand.Name!,
				ProductType = ProductType.Book,
				AuthorId = givenAuthorId2,
				RetailPrice = givenPatchBookCommand.RetailPrice!.Value,
				Price = givenPatchBookCommand.Price!.Value,
				DiscountAmmount = givenPatchBookCommand.DiscountAmmount!.Value,
				Description = givenPatchBookCommand.Description,
				CoverImageUrl = givenPatchBookCommand.CoverImageUrl,
				ProductCategoryIds = givenPatchBookCommand.ProductCategoryIds ?? []
			});
	}

	[Fact]
	public async Task PatchBook_should_update_single_property_book_when_user_is_Admin()
	{
		// Given
		AuthenticateAdmin();
		var givenAuthorId = await _bookStoreDriver.SeedAuthor(_fixture.Create<CreateAuthorCommand>());
		var givenCreateBookSeedCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.AuthorId, givenAuthorId)
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenBookId = await _bookStoreDriver.SeedBook(givenCreateBookSeedCommand);
		var givenPatchBookCommand = new PatchBookCommandDto(
			Name: null,
			AuthorId: null,
			RetailPrice: null,
			Price: null,
			DiscountAmmount: null,
			Description: "patched description",
			CoverImageUrl: null,
			ProductCategoryIds: null);

		// When
		var response = await _bookStoreApi.PatchBook(givenBookId, givenPatchBookCommand);
		var actualBook = await _bookStoreApi.GetProductById<Book>(givenBookId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		actualBook.Should().BeEquivalentTo(
			new Book
			{
				Id = givenBookId,
				Name = givenCreateBookSeedCommand.Name,
				ProductType = ProductType.Book,
				AuthorId = givenAuthorId,
				RetailPrice = givenCreateBookSeedCommand.RetailPrice,
				Price = givenCreateBookSeedCommand.Price,
				DiscountAmmount = givenCreateBookSeedCommand.DiscountAmmount,
				Description = givenPatchBookCommand.Description,
				CoverImageUrl = givenCreateBookSeedCommand.CoverImageUrl,
				ProductCategoryIds = givenCreateBookSeedCommand.ProductCategoryIds ?? []
			});
	}

	[Fact]
	public async Task PatchBook_should_return_401_unauthorized_when_user_is_not_authorized()
	{
		// Given
		// When
		var response = await _bookStoreApi.PatchBook(Guid.NewGuid(), _fixture.Create<PatchBookCommandDto>());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}

	[Fact]
	public async Task PatchBook_should_return_403_forbidden_when_user_is_standard_user()
	{
		// Given
		AuthenticateUser();

		// When
		var response = await _bookStoreApi.PatchBook(Guid.NewGuid(), _fixture.Create<PatchBookCommandDto>());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
	}

	#endregion

	private Guid? GetGuidFromResponseLocation(HttpResponseMessage response)
	{
		var success = Guid.TryParse(response.Headers.Location?.Segments[^1], out var result);
		return success ? result : null;
	}
}
