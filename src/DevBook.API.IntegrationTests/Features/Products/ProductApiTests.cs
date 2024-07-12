using AutoFixture;
using DevBook.API.Features.BookStore.Authors;
using DevBook.API.Features.BookStore.Products;
using DevBook.API.Features.BookStore.Products.Books;
using System.Net;

namespace DevBook.API.IntegrationTests.Features.Products;

public class ProductApiTests : IntegrationTestsBase
{
	private readonly IBookStoreApi _bookStoreApi;
	private readonly BookStoreApiDriver _bookStoreDriver;
	private readonly Fixture _fixture = new();

	public ProductApiTests(ITestOutputHelper outputHelper) : base(outputHelper)
	{
		_bookStoreApi = GetClient<IBookStoreApi>();
		_bookStoreDriver = new BookStoreApiDriver(_bookStoreApi, TestAuthInterceptor);
	}

	#region GetProducts

	[Fact]
	public async Task GetProducts_should_return_Products()
	{
		// Given
		var givenAuthorId = await _bookStoreDriver.SeedAuthor(_fixture.Create<CreateAuthorCommand>());
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.AuthorId, givenAuthorId)
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenBookId = await _bookStoreDriver.SeedBook(givenCreateBookCommand);

		// When
		var response = await _bookStoreApi.GetProducts<Book>();

		// Then
		response.Should().NotBeNull();
		response.Should().NotBeEmpty();
		response.Count.Should().Be(1);
		response.First().Should().BeEquivalentTo(
			new Book
			{
				Id = givenBookId,
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

	#endregion

	#region GetProductById

	[Fact]
	public async Task GetProductById_should_return_Product()
	{
		// Given
		var givenAuthorId = await _bookStoreDriver.SeedAuthor(_fixture.Create<CreateAuthorCommand>());
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.AuthorId, givenAuthorId)
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenBookId = await _bookStoreDriver.SeedBook(givenCreateBookCommand);

		// When
		var response = await _bookStoreApi.GetProductById<Book>(givenBookId);

		// Then
		response.Should().NotBeNull();
		response.Should().BeEquivalentTo(
			new Book
			{
				Id = givenBookId,
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

	#endregion

	#region DeleteProduct

	[Fact]
	public async Task DeleteProduct_should_succeed_for_Admin_user()
	{
		// Given
		AuthenticateAdmin();
		var givenAuthorId = await _bookStoreDriver.SeedAuthor(_fixture.Create<CreateAuthorCommand>());
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.AuthorId, givenAuthorId)
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenBookId = await _bookStoreDriver.SeedBook(givenCreateBookCommand);

		// When
		var response = await _bookStoreApi.DeleteProduct(givenBookId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task DeleteProduct_should_succeed_for_Admin_user_when_product_does_not_exist()
	{
		// Given
		AuthenticateAdmin();

		// When
		var response = await _bookStoreApi.DeleteProduct(Guid.NewGuid());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task DeleteProduct_should_return_403_forbidden_for_standard_user()
	{
		// Given
		AuthenticateUser();

		// When
		var response = await _bookStoreApi.DeleteProduct(Guid.NewGuid());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
	}

	[Fact]
	public async Task DeleteProduct_should_return_401_unauthorized_when_use_is_not_authenticated()
	{
		// Given
		// When
		var response = await _bookStoreApi.DeleteProduct(Guid.NewGuid());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}

	#endregion

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
