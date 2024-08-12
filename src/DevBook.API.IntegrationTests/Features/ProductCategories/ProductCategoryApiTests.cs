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
		var givenCreateProductCategoryCommand1 = _fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.Subcategories, [])
			.Create();
		var givenCreateProductCategoryCommand2 = _fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.Subcategories, [])
			.Create();
		var givenProductCategoryId1 = await _bookStoreDriver.SeedProductCategory(givenCreateProductCategoryCommand1);
		var givenProductCategoryId2 = await _bookStoreDriver.SeedProductCategory(givenCreateProductCategoryCommand2);

		// When
		var response = await _bookStoreApi.GetProductCategories();

		// Then
		response.Should().NotBeNull();
		response.Should().NotBeEmpty();
		response.Count.Should().Be(2);
		response.Should().BeEquivalentTo([
			new ProductCategoryDto
			{
				Id = givenProductCategoryId1,
				Name = givenCreateProductCategoryCommand1.Name,
				IsTopLevelCategory = givenCreateProductCategoryCommand1.IsTopLevelCategory!.Value,
				Subcategories = []
			},
			new ProductCategoryDto
			{
				Id = givenProductCategoryId2,
				Name = givenCreateProductCategoryCommand2.Name,
				IsTopLevelCategory = givenCreateProductCategoryCommand2.IsTopLevelCategory!.Value,
				Subcategories = []
			}
		]);
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
		var givenCreateProductCategoryCommand = _fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.Subcategories, [])
			.Create();
		var givenProductCategoryId = await _bookStoreDriver.SeedProductCategory(givenCreateProductCategoryCommand);

		// When
		var response = await _bookStoreApi.GetProductCategoryById(givenProductCategoryId);

		// Then
		response.Should().NotBeNull();
		response.Should().BeEquivalentTo(
			new ProductCategoryDto
			{
				Id = givenProductCategoryId,
				Name = givenCreateProductCategoryCommand.Name,
				IsTopLevelCategory = givenCreateProductCategoryCommand.IsTopLevelCategory!.Value,
				Subcategories = []
			});
	}

	[Fact]
	public async Task GetProductCategoryById_should_return_category_with_subcategory()
	{
		// Given
		var givenCreateProductCategoryCommand1 = _fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.Subcategories, [])
			.Create();
		var givenProductCategoryId1 = await _bookStoreDriver.SeedProductCategory(givenCreateProductCategoryCommand1);

		var givenCreateProductCategoryCommand2 = _fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.Subcategories, [givenProductCategoryId1])
			.Create();
		var givenProductCategoryId2 = await _bookStoreDriver.SeedProductCategory(givenCreateProductCategoryCommand2);

		// When
		var response = await _bookStoreApi.GetProductCategoryById(givenProductCategoryId2);

		// Then
		response.Should().NotBeNull();
		response.Should().BeEquivalentTo(
			new ProductCategoryDto
			{
				Id = givenProductCategoryId2,
				Name = givenCreateProductCategoryCommand2.Name,
				IsTopLevelCategory = false,
				Subcategories = [givenCreateProductCategoryCommand1.Name],
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
		var givenCreateProductCategoryCommand = _fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.Subcategories, [])
			.Create();

		// When
		var response = await _bookStoreApi.CreateProductCategory(givenCreateProductCategoryCommand);
		var actualCategory = (await _bookStoreApi.GetProductCategories()).Single();

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Created);
		var actualCategoryId = response.GetGuidFromResponseLocation();
		actualCategoryId.Should().NotBeNull();
		actualCategory.Should().BeEquivalentTo(
			new ProductCategoryDto
			{
				Id = actualCategoryId!.Value,
				Name = givenCreateProductCategoryCommand.Name,
				IsTopLevelCategory = givenCreateProductCategoryCommand.IsTopLevelCategory!.Value,
				Subcategories = [],
			});
	}

	[Fact]
	public async Task CreateProductCategory_should_create_top_level_category_with_subcategory()
	{
		// Given
		AuthenticateAdmin();
		var givenCreateProductCategoryCommand1 = _fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.Subcategories, [])
			.Create();
		var givenProductCategoryId1 = await _bookStoreDriver.SeedProductCategory(givenCreateProductCategoryCommand1);

		var givenCreateProductCategoryCommand2 = _fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.IsTopLevelCategory, true)
			.With(x => x.Subcategories, [givenProductCategoryId1])
			.Create();

		// When
		var response = await _bookStoreApi.CreateProductCategory(givenCreateProductCategoryCommand2);
		var actualCategories = (await _bookStoreApi.GetProductCategories());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Created);
		var actualCreatedCategoryId = response.GetGuidFromResponseLocation();
		actualCreatedCategoryId.Should().NotBeNull();

		actualCategories.Count.Should().Be(2);
		actualCategories.Single(x => x.Id == actualCreatedCategoryId).Should().BeEquivalentTo(
			new ProductCategoryDto
			{
				Id = actualCreatedCategoryId!.Value,
				Name = givenCreateProductCategoryCommand2.Name,
				IsTopLevelCategory = givenCreateProductCategoryCommand2.IsTopLevelCategory!.Value,
				Subcategories = [givenCreateProductCategoryCommand1.Name],
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
		var givenProductCategoryId = await _bookStoreDriver.SeedProductCategory(_fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.Subcategories, [])
			.Create());
		var givenUpdateProductCategoryCommand = _fixture
			.Build<UpdateProductCategoryCommandDto>()
			.With(x => x.Subcategories, [])
			.Create();

		// When
		var response = await _bookStoreApi.UpdateProductCategory(givenProductCategoryId, givenUpdateProductCategoryCommand);
		var actualCategory = await _bookStoreApi.GetProductCategoryById(givenProductCategoryId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		actualCategory.Should().BeEquivalentTo(
			new ProductCategoryDto
			{
				Id = givenProductCategoryId,
				Name = givenUpdateProductCategoryCommand.Name,
				IsTopLevelCategory = givenUpdateProductCategoryCommand.IsTopLevelCategory,
				Subcategories = [],
			});
	}

	[Fact]
	public async Task UpdateProductCategory_should_update_category_subcategories()
	{
		// Given
		AuthenticateAdmin();
		var givenCreateProductCategoryCommand = _fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.Subcategories, [])
			.Create();
		var givenProductCategoryId1 = await _bookStoreDriver.SeedProductCategory(givenCreateProductCategoryCommand);

		var givenProductCategoryId2 = await _bookStoreDriver.SeedProductCategory(_fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.Subcategories, [])
			.Create());

		var givenUpdateProductCategoryCommand = _fixture
			.Build<UpdateProductCategoryCommandDto>()
			.With(x => x.Subcategories, [givenProductCategoryId1])
			.Create();

		// When
		var response = await _bookStoreApi.UpdateProductCategory(givenProductCategoryId2, givenUpdateProductCategoryCommand);
		var actualCategory = await _bookStoreApi.GetProductCategoryById(givenProductCategoryId2);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		actualCategory.Should().BeEquivalentTo(
			new ProductCategoryDto
			{
				Id = givenProductCategoryId2,
				Name = givenUpdateProductCategoryCommand.Name,
				IsTopLevelCategory = givenUpdateProductCategoryCommand.IsTopLevelCategory,
				Subcategories = [givenCreateProductCategoryCommand.Name],
			});
	}

	[Fact]
	public async Task UpdateProductCategory_should_return_400_badRequest_when_subcategories_contain_same_id_as_updated_category()
	{
		// Given
		AuthenticateAdmin();
		var givenProductCategoryId1 = await _bookStoreDriver.SeedProductCategory(_fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.Subcategories, [])
			.Create());

		var givenUpdateProductCategoryCommand = _fixture
			.Build<UpdateProductCategoryCommandDto>()
			.With(x => x.Subcategories, [givenProductCategoryId1])
			.Create();

		// When

		var response = await _bookStoreApi.UpdateProductCategory(givenProductCategoryId1, givenUpdateProductCategoryCommand);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	[Fact]
	public async Task UpdateProductCategory_should_return_401_unauthorized_when_user_is_not_authorized()
	{
		// Given
		// When
		var response = await _bookStoreApi.UpdateProductCategory(Guid.NewGuid(), _fixture.Create<UpdateProductCategoryCommandDto>());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}

	[Fact]
	public async Task UpdateProductCategory_should_return_403_forbidden_when_user_is_standard_user()
	{
		// Given
		AuthenticateUser();

		// When
		var response = await _bookStoreApi.UpdateProductCategory(Guid.NewGuid(), _fixture.Create<UpdateProductCategoryCommandDto>());

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
		var givenProductCategoryId = await _bookStoreDriver.SeedProductCategory(_fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.Subcategories, [])
			.Create());

		// When
		var response = await _bookStoreApi.DeleteProductCategory(givenProductCategoryId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task DeleteProductCategory_should_succeed_when_product_does_not_exist()
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
		var givenProductCategoryId = await _bookStoreDriver.SeedProductCategory(_fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.Subcategories, [])
			.Create());
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
