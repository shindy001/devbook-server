using AutoFixture;
using DevBook.API.Features.BookStore.Authors;
using DevBook.API.Features.BookStore.Products;
using DevBook.API.Features.BookStore.Products.Books;

namespace DevBook.API.IntegrationTests.Features.Products;

public class ProductApiTests : IntegrationTestsBase
{
	private readonly IBookStoreApi _bookStoreApi;
	private readonly Fixture _fixture = new Fixture();

	public ProductApiTests(ITestOutputHelper outputHelper) : base(outputHelper)
	{
		_bookStoreApi = this.GetClient<IBookStoreApi>();
	}

	[Fact]
	public async Task GetProducts_should_return_Products()
	{
		// Given
		this.AuthenticatedAdmin();
		var givenAuthorId = await _bookStoreApi.SeedAuthor(_fixture.Create<CreateAuthorCommand>());
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.AuthorId, givenAuthorId)
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenBookId = await _bookStoreApi.SeedBook(givenCreateBookCommand);

		// When
		var response = await _bookStoreApi.GetProducts<Book>();

		// Then
		response.Should().NotBeNull();
		response.Should().NotBeEmpty();
		response.Count.Should().Be(1);
		(response.First()).Should().BeEquivalentTo(
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
}
