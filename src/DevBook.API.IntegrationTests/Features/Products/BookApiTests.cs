using AutoFixture;
using DevBook.API.Features.BookStore.Products;
using DevBook.API.Features.BookStore.Products.Books;
using DevBook.API.IntegrationTests.Extensions;
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
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.ProductCategoryIds, [])
			.Create();

		// When
		var response = await _bookStoreApi.CreateBook(givenCreateBookCommand);
		var actualBook = (await _bookStoreApi.GetProducts<BookDto>()).Single();

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Created);
		var actualBookId = response.GetGuidFromResponseLocation();
		actualBookId.Should().NotBeNull();
		actualBook.Should().BeEquivalentTo(
			new BookDto
			{
				Id = actualBookId!.Value,
				Name = givenCreateBookCommand.Name,
				ProductType = ProductType.Book,
				RetailPrice = givenCreateBookCommand.RetailPrice,
				Price = givenCreateBookCommand.Price,
				DiscountAmmount = givenCreateBookCommand.DiscountAmmount,
				Author = givenCreateBookCommand.Author,
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
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.Name, string.Empty)
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
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
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
		var givenCreateBookSeedCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenBookId = await _bookStoreDriver.SeedBook(givenCreateBookSeedCommand);
		var givenUpdateBookCommand = _fixture
			.Build<UpdateBookCommandDto>()
			.With(x => x.ProductCategoryIds, [])
			.Create();

		// When
		var response = await _bookStoreApi.UpdateBook(givenBookId, givenUpdateBookCommand);
		var actualBook = await _bookStoreApi.GetProductById<BookDto>(givenBookId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		actualBook.Should().BeEquivalentTo(
			new BookDto
			{
				Id = givenBookId,
				Name = givenUpdateBookCommand.Name,
				ProductType = ProductType.Book,
				RetailPrice = givenUpdateBookCommand.RetailPrice,
				Price = givenUpdateBookCommand.Price,
				DiscountAmmount = givenUpdateBookCommand.DiscountAmmount,
				Author = givenUpdateBookCommand.Author,
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
		var givenCreateBookSeedCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenBookId = await _bookStoreDriver.SeedBook(givenCreateBookSeedCommand);
		var givenPatchBookCommand = _fixture
			.Build<PatchBookCommandDto>()
			.With(x => x.ProductCategoryIds, [])
			.Create();

		// When
		var response = await _bookStoreApi.PatchBook(givenBookId, givenPatchBookCommand);
		var actualBook = await _bookStoreApi.GetProductById<BookDto>(givenBookId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		actualBook.Should().BeEquivalentTo(
			new BookDto
			{
				Id = givenBookId,
				Name = givenPatchBookCommand.Name!,
				ProductType = ProductType.Book,
				RetailPrice = givenPatchBookCommand.RetailPrice!.Value,
				Price = givenPatchBookCommand.Price!.Value,
				DiscountAmmount = givenPatchBookCommand.DiscountAmmount!.Value,
				Author = givenPatchBookCommand.Author,
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
		var givenCreateBookSeedCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenBookId = await _bookStoreDriver.SeedBook(givenCreateBookSeedCommand);
		var givenPatchBookCommand = new PatchBookCommandDto(Description: "patched description");

		// When
		var response = await _bookStoreApi.PatchBook(givenBookId, givenPatchBookCommand);
		var actualBook = await _bookStoreApi.GetProductById<BookDto>(givenBookId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		actualBook.Should().BeEquivalentTo(
			new BookDto
			{
				Id = givenBookId,
				Name = givenCreateBookSeedCommand.Name,
				ProductType = ProductType.Book,
				RetailPrice = givenCreateBookSeedCommand.RetailPrice,
				Price = givenCreateBookSeedCommand.Price,
				DiscountAmmount = givenCreateBookSeedCommand.DiscountAmmount,
				Author = givenCreateBookSeedCommand.Author,
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
}
