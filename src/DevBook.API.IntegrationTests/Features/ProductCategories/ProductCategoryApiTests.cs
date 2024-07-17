using AutoFixture;
using DevBook.API.Features.BookStore.Authors;
using DevBook.API.Features.BookStore.ProductCategories;
using DevBook.API.Features.BookStore.Products.Books;
using DevBook.API.IntegrationTests.Extensions;
using System.Net;

namespace DevBook.API.IntegrationTests.Features.ProductCategories;

public class ProductCategoryApiTests : IntegrationTestsBase
{
	private readonly IBookStoreApi _bookStoreApi;
	private readonly BookStoreApiDriver _bookStoreDriver;
	private readonly Fixture _fixture = new();

	public ProductCategoryApiTests(ITestOutputHelper outputHelper) : base(outputHelper)
	{
		_bookStoreApi = GetClient<IBookStoreApi>();
		_bookStoreDriver = new BookStoreApiDriver(_bookStoreApi, TestAuthInterceptor);
	}

	#region GetProductCategories

	[Fact]
	public async Task GetProductCategories_should_return_categories()
	{
		// Given
		var givenCreateProductCategoryCommand = _fixture.Create<CreateProductCategoryCommand>();
		var givenProductCategoryId = await _bookStoreDriver.SeedProductCategory(givenCreateProductCategoryCommand);

		// When
		var response = await _bookStoreApi.GetProductCategories();

		// Then
		response.Should().NotBeNull();
		response.Should().NotBeEmpty();
		response.Count.Should().Be(1);
		response.First().Should().BeEquivalentTo(
			new ProductCategory
			{
				Id = givenProductCategoryId,
				Name = givenCreateProductCategoryCommand.Name,
			});
	}

	[Fact]
	public async Task GetProductCategories_should_return_empty_collection_when_there_are_no_categories()
	{
		// Given
		// When
		var response = await _bookStoreApi.GetProductCategories();

		// Then
		response.Should().NotBeNull();
		response.Should().BeEmpty();
	}

	#endregion

	#region GetProductCategoryById

	[Fact]
	public async Task GetProductCategoryById_should_return_category()
	{
		// Given
		var givenCreateProductCategoryCommand = _fixture.Create<CreateProductCategoryCommand>();
		var givenProductCategoryId = await _bookStoreDriver.SeedProductCategory(givenCreateProductCategoryCommand);

		// When
		var response = await _bookStoreApi.GetProductCategoryById(givenProductCategoryId);

		// Then
		response.Should().NotBeNull();
		response.Should().BeEquivalentTo(
			new ProductCategory
			{
				Id = givenProductCategoryId,
				Name = givenCreateProductCategoryCommand.Name,
			});
	}

	[Fact]
	public async Task GetProductCategoryById_should_throw_404_NotFound_ApiException_when_category_does_not_exist()
	{
		// Given
		// When
		Func<Task> act = async () => await _bookStoreApi.GetProductCategoryById(Guid.NewGuid());

		// Then
		var exception = await act.Should().ThrowAsync<ApiException>();
		exception.Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	#endregion

	#region CreateProductCategory

	[Fact]
	public async Task CreateProductCategory_should_create_category_when_user_is_Admin()
	{
		// Given
		AuthenticateAdmin();
		var givenCreateProductCategoryCommand = _fixture.Create<CreateProductCategoryCommand>();

		// When
		var response = await _bookStoreApi.CreateProductCategory(givenCreateProductCategoryCommand);
		var actualCategory = (await _bookStoreApi.GetProductCategories()).Single();

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Created);
		var actualCategoryId = response.GetGuidFromResponseLocation();
		actualCategoryId.Should().NotBeNull();
		actualCategory.Should().BeEquivalentTo(
			new ProductCategory
			{
				Id = actualCategoryId!.Value,
				Name = givenCreateProductCategoryCommand.Name,
			});
	}

	[Fact]
	public async Task CreateProductCategory_should_return_401_unauthorized_when_user_is_not_authenticated()
	{
		// Given
		var givenCreateProductCategoryCommand = _fixture.Create<CreateProductCategoryCommand>();

		// When
		var response = await _bookStoreApi.CreateProductCategory(givenCreateProductCategoryCommand);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}

	[Fact]
	public async Task CreateProductCategory_should_return_403_forbidden_when_standard_user_is_authenticated()
	{
		// Given
		AuthenticateUser();
		var givenCreateProductCategoryCommand = _fixture.Create<CreateProductCategoryCommand>();

		// When
		var response = await _bookStoreApi.CreateProductCategory(givenCreateProductCategoryCommand);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
	}

	[Fact]
	public async Task CreateProductCategory_should_return_400_badRequest_when_create_command_has_empty_name()
	{
		// Given
		AuthenticateAdmin();
		var givenCreateProductCategoryCommand = _fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.Name, string.Empty)
			.Create();


		// When
		var response = await _bookStoreApi.CreateProductCategory(givenCreateProductCategoryCommand);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	#endregion

	#region UpdateProductCategory

	[Fact]
	public async Task UpdateProductCategory_should_update_category_when_user_is_Admin()
	{
		// Given
		AuthenticateAdmin();
		var givenProductCategoryId = await _bookStoreDriver.SeedProductCategory(_fixture.Create<CreateProductCategoryCommand>());
		var givenUpdateProductCategoryCommand = _fixture.Create<UpdateProductCategoryCommand>();

		// When
		var response = await _bookStoreApi.UpdateProductCategory(givenProductCategoryId, givenUpdateProductCategoryCommand);
		var actualCategory = await _bookStoreApi.GetProductCategoryById(givenProductCategoryId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		actualCategory.Should().BeEquivalentTo(
			new ProductCategory
			{
				Id = givenProductCategoryId,
				Name = givenUpdateProductCategoryCommand.Name,
			});
	}

	[Fact]
	public async Task UpdateProductCategory_should_return_401_unauthorized_when_user_is_not_authorized()
	{
		// Given
		// When
		var response = await _bookStoreApi.UpdateProductCategory(Guid.NewGuid(), _fixture.Create<UpdateProductCategoryCommand>());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}

	[Fact]
	public async Task UpdateProductCategory_should_return_403_forbidden_when_user_is_standard_user()
	{
		// Given
		AuthenticateUser();

		// When
		var response = await _bookStoreApi.UpdateProductCategory(Guid.NewGuid(), _fixture.Create<UpdateProductCategoryCommand>());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
	}

	#endregion

	#region DeleteProductCategory

	[Fact]
	public async Task DeleteProductCategory_should_succeed_for_Admin_user()
	{
		// Given
		AuthenticateAdmin();
		var givenProductCategoryId = await _bookStoreDriver.SeedProductCategory(_fixture.Create<CreateProductCategoryCommand>());

		// When
		var response = await _bookStoreApi.DeleteProductCategory(givenProductCategoryId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task DeleteProductCategory_should_succeed_for_Admin_user_when_product_does_not_exist()
	{
		// Given
		AuthenticateAdmin();

		// When
		var response = await _bookStoreApi.DeleteProductCategory(Guid.NewGuid());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task DeleteProductCategory_should_return_400_badRequest_when_category_is_used_on_some_product()
	{
		// Given
		AuthenticateAdmin();
		var givenAuthorId = await _bookStoreDriver.SeedAuthor(_fixture.Create<CreateAuthorCommand>());
		var givenProductCategoryId = await _bookStoreDriver.SeedProductCategory(_fixture.Create<CreateProductCategoryCommand>());
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.AuthorId, givenAuthorId)
			.With(x => x.ProductCategoryIds, [givenProductCategoryId])
			.Create();
		var givenBookId = await _bookStoreDriver.SeedBook(givenCreateBookCommand);


		// When
		var response = await _bookStoreApi.DeleteProductCategory(givenProductCategoryId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	[Fact]
	public async Task DeleteProductCategory_should_return_403_forbidden_for_standard_user()
	{
		// Given
		AuthenticateUser();

		// When
		var response = await _bookStoreApi.DeleteProductCategory(Guid.NewGuid());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
	}

	[Fact]
	public async Task DeleteProductCategory_should_return_401_unauthorized_when_use_is_not_authenticated()
	{
		// Given
		// When
		var response = await _bookStoreApi.DeleteProductCategory(Guid.NewGuid());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}

	#endregion
}
