using AutoFixture;
using DevBook.API.Features.BookStore.Authors;
using DevBook.API.Features.BookStore.Products.Books;
using DevBook.API.IntegrationTests.Extensions;
using System.Net;

namespace DevBook.API.IntegrationTests.Features.Authors;

public class AuthorsApiTests : IntegrationTestsBase
{
	private readonly IBookStoreApi _bookStoreApi;
	private readonly BookStoreApiDriver _bookStoreDriver;
	private readonly Fixture _fixture = new();

	public AuthorsApiTests(ITestOutputHelper outputHelper) : base(outputHelper)
	{
		_bookStoreApi = GetClient<IBookStoreApi>();
		_bookStoreDriver = new BookStoreApiDriver(_bookStoreApi, TestAuthInterceptor);
	}

	#region GetAuthors

	[Fact]
	public async Task GetAuthors_should_return_authors()
	{
		// Given
		var givenCreateAuthorCommand = _fixture.Create<CreateAuthorCommand>();
		var givenAuthorId = await _bookStoreDriver.SeedAuthor(givenCreateAuthorCommand);

		// When
		var response = await _bookStoreApi.GetAuthors();

		// Then
		response.Should().NotBeNull();
		response.Should().NotBeEmpty();
		response.Count.Should().Be(1);
		response.First().Should().BeEquivalentTo(
			new Author
			{
				Id = givenAuthorId,
				Name = givenCreateAuthorCommand.Name,
				Description = givenCreateAuthorCommand.Description,
			});
	}

	[Fact]
	public async Task GetAuthors_should_return_empty_collection_when_there_are_no_authors()
	{
		// Given
		// When
		var response = await _bookStoreApi.GetAuthors();

		// Then
		response.Should().NotBeNull();
		response.Should().BeEmpty();
	}

	#endregion

	#region GetAuthorById

	[Fact]
	public async Task GetAuthorById_should_return_author()
	{
		// Given
		var givenCreateAuthorCommand = _fixture.Create<CreateAuthorCommand>();
		var givenAuthorId = await _bookStoreDriver.SeedAuthor(givenCreateAuthorCommand);

		// When
		var response = await _bookStoreApi.GetAuthorById(givenAuthorId);

		// Then
		response.Should().NotBeNull();
		response.Should().BeEquivalentTo(
			new Author
			{
				Id = givenAuthorId,
				Name = givenCreateAuthorCommand.Name,
				Description = givenCreateAuthorCommand.Description,
			});
	}

	[Fact]
	public async Task GetAuthorById_should_throw_404_NotFound_ApiException_when_author_does_not_exist()
	{
		// Given
		// When
		Func<Task> act = async () => await _bookStoreApi.GetAuthorById(Guid.NewGuid());

		// Then
		var exception = await act.Should().ThrowAsync<ApiException>();
		exception.Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	#endregion

	#region CreateAuthor

	[Fact]
	public async Task CreateAuthor_should_create_author_when_user_is_Admin()
	{
		// Given
		AuthenticateAdmin();
		var givenCreateAuthorCommand = _fixture.Create<CreateAuthorCommand>();

		// When
		var response = await _bookStoreApi.CreateAuthor(givenCreateAuthorCommand);
		var actualAuthor = (await _bookStoreApi.GetAuthors()).Single();

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Created);
		var actualAuthorId = response.GetGuidFromResponseLocation();
		actualAuthorId.Should().NotBeNull();
		actualAuthor.Should().BeEquivalentTo(
			new Author
			{
				Id = actualAuthorId!.Value,
				Name = givenCreateAuthorCommand.Name,
				Description = givenCreateAuthorCommand.Description,
			});
	}

	[Fact]
	public async Task CreateAuthor_should_return_401_unauthorized_when_user_is_not_authenticated()
	{
		// Given
		var givenCreateAuthorCommand = _fixture.Create<CreateAuthorCommand>();

		// When
		var response = await _bookStoreApi.CreateAuthor(givenCreateAuthorCommand);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}

	[Fact]
	public async Task CreateAuthor_should_return_403_forbidden_when_standard_user_is_authenticated()
	{
		// Given
		AuthenticateUser();
		var givenCreateAuthorCommand = _fixture.Create<CreateAuthorCommand>();

		// When
		var response = await _bookStoreApi.CreateAuthor(givenCreateAuthorCommand);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
	}

	[Fact]
	public async Task CreateAuthor_should_return_400_badRequest_when_create_command_has_empty_name()
	{
		// Given
		AuthenticateAdmin();
		var givenCreateAuthorCommand = _fixture
			.Build<CreateAuthorCommand>()
			.With(x => x.Name, string.Empty)
			.Create();


		// When
		var response = await _bookStoreApi.CreateAuthor(givenCreateAuthorCommand);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	#endregion

	#region UpdateAuthor

	[Fact]
	public async Task UpdateAuthor_should_update_category_when_user_is_Admin()
	{
		// Given
		AuthenticateAdmin();
		var givenAuthorId = await _bookStoreDriver.SeedAuthor(_fixture.Create<CreateAuthorCommand>());
		var givenUpdateAuthorCommand = _fixture.Create<UpdateAuthorCommandDto>();

		// When
		var response = await _bookStoreApi.UpdateAuthor(givenAuthorId, givenUpdateAuthorCommand);
		var actualAuthor = await _bookStoreApi.GetAuthorById(givenAuthorId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		actualAuthor.Should().BeEquivalentTo(
			new Author
			{
				Id = givenAuthorId,
				Name = givenUpdateAuthorCommand.Name,
				Description = givenUpdateAuthorCommand.Description,
			});
	}

	[Fact]
	public async Task UpdateAuthor_should_return_401_unauthorized_when_user_is_not_authorized()
	{
		// Given
		// When
		var response = await _bookStoreApi.UpdateAuthor(Guid.NewGuid(), _fixture.Create<UpdateAuthorCommandDto>());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}

	[Fact]
	public async Task UpdateAuthor_should_return_403_forbidden_when_user_is_standard_user()
	{
		// Given
		AuthenticateUser();

		// When
		var response = await _bookStoreApi.UpdateAuthor(Guid.NewGuid(), _fixture.Create<UpdateAuthorCommandDto>());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
	}

	#endregion

	#region PatchAuthor

	[Fact]
	public async Task PatchAuthor_should_update_author_when_user_is_Admin()
	{
		// Given
		AuthenticateAdmin();
		var givenAuthorId = await _bookStoreDriver.SeedAuthor(_fixture.Create<CreateAuthorCommand>());
		var givenPatchAuthorCommand = _fixture
			.Build<PatchAuthorCommandDto>()
			.Create();

		// When
		var response = await _bookStoreApi.PatchAuthor(givenAuthorId, givenPatchAuthorCommand);
		var actualAuthor = await _bookStoreApi.GetAuthorById(givenAuthorId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		actualAuthor.Should().BeEquivalentTo(
			new Author
			{
				Id = givenAuthorId,
				Name = givenPatchAuthorCommand.Name!,
				Description = givenPatchAuthorCommand.Description,
			});
	}

	[Fact]
	public async Task PatchAuthor_should_update_single_property_book_when_user_is_Admin()
	{
		// Given
		AuthenticateAdmin();
		var givenCreateAuthorCommand = _fixture.Create<CreateAuthorCommand>();
		var givenAuthorId = await _bookStoreDriver.SeedAuthor(givenCreateAuthorCommand);
		var givenPatchAuthorCommand = new PatchAuthorCommandDto(Description: "patched description");

		// When
		var response = await _bookStoreApi.PatchAuthor(givenAuthorId, givenPatchAuthorCommand);
		var actualAuthor = await _bookStoreApi.GetAuthorById(givenAuthorId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		actualAuthor.Should().BeEquivalentTo(
			new Author
			{
				Id = givenAuthorId,
				Name = givenCreateAuthorCommand.Name,
				Description = givenPatchAuthorCommand.Description,
			});
	}

	[Fact]
	public async Task PatchAuthor_should_return_401_unauthorized_when_user_is_not_authorized()
	{
		// Given
		// When
		var response = await _bookStoreApi.PatchAuthor(Guid.NewGuid(), _fixture.Create<PatchAuthorCommandDto>());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}

	[Fact]
	public async Task PatchAuthor_should_return_403_forbidden_when_user_is_standard_user()
	{
		// Given
		AuthenticateUser();

		// When
		var response = await _bookStoreApi.PatchAuthor(Guid.NewGuid(), _fixture.Create<PatchAuthorCommandDto>());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
	}

	#endregion

	#region DeleteAuthor

	[Fact]
	public async Task DeleteAuthorCategory_should_succeed_for_Admin_user()
	{
		// Given
		AuthenticateAdmin();
		var givenProductCategoryId = await _bookStoreDriver.SeedAuthor(_fixture.Create<CreateAuthorCommand>());

		// When
		var response = await _bookStoreApi.DeleteAuthor(givenProductCategoryId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task DeleteAuthor_should_succeed_for_Admin_user_when_product_does_not_exist()
	{
		// Given
		AuthenticateAdmin();

		// When
		var response = await _bookStoreApi.DeleteAuthor(Guid.NewGuid());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task DeleteAuthor_should_return_400_badRequest_when_category_is_used_on_some_product()
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
		var response = await _bookStoreApi.DeleteAuthor(givenAuthorId);

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	[Fact]
	public async Task DeleteAuthor_should_return_403_forbidden_for_standard_user()
	{
		// Given
		AuthenticateUser();

		// When
		var response = await _bookStoreApi.DeleteAuthor(Guid.NewGuid());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
	}

	[Fact]
	public async Task DeleteAuthor_should_return_401_unauthorized_when_use_is_not_authenticated()
	{
		// Given
		// When
		var response = await _bookStoreApi.DeleteAuthor(Guid.NewGuid());

		// Then
		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}

	#endregion
}
