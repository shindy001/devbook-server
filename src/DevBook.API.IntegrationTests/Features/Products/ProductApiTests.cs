﻿using AutoFixture;
using DevBook.API.Features.BookStore.ProductCategories;
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
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenBookId = await _bookStoreDriver.SeedBook(givenCreateBookCommand);

		// When
		var response = await _bookStoreApi.GetProducts<BookDto>();

		// Then
		response.Should().NotBeNull();
		response.Should().NotBeEmpty();
		response.Count.Should().Be(1);
		response.First().Should().BeEquivalentTo(
			new BookDto
			{
				Id = givenBookId,
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
	public async Task GetProducts_should_return_Products_with_specified_ProductType()
	{
		// Given
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenBookId = await _bookStoreDriver.SeedBook(givenCreateBookCommand);

		// When
		var response = await _bookStoreApi.GetProducts<BookDto>(new GetProductsQuery(ProductType: ProductType.Book));

		// Then
		response.Should().NotBeNull();
		response.Should().NotBeEmpty();
		response.Count.Should().Be(1);
		response.First().Should().BeEquivalentTo(
			new BookDto
			{
				Id = givenBookId,
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
	public async Task GetProducts_should_return_Products_with_ProductCategory()
	{
		// Given
		var givenProductCategoryId = await _bookStoreDriver.SeedProductCategory(_fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.Subcategories, [])
			.Create());
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.ProductCategoryIds, [givenProductCategoryId])
			.Create();
		var givenBookId = await _bookStoreDriver.SeedBook(givenCreateBookCommand);
		await _bookStoreDriver.SeedBook(givenCreateBookCommand with { ProductCategoryIds = [] });

		// When
		var response = await _bookStoreApi.GetProducts<BookDto>(new GetProductsQuery(ProductCategoryId: givenProductCategoryId));

		// Then
		response.Should().NotBeNull();
		response.Should().NotBeEmpty();
		response.Count.Should().Be(1);
		response.First().Should().BeEquivalentTo(
			new BookDto
			{
				Id = givenBookId,
				Name = givenCreateBookCommand.Name,
				ProductType = ProductType.Book,
				RetailPrice = givenCreateBookCommand.RetailPrice,
				Price = givenCreateBookCommand.Price,
				DiscountAmmount = givenCreateBookCommand.DiscountAmmount,
				Author = givenCreateBookCommand.Author,
				Description = givenCreateBookCommand.Description,
				CoverImageUrl = givenCreateBookCommand.CoverImageUrl,
				ProductCategoryIds = [givenProductCategoryId]
			});
	}

	[Fact]
	public async Task GetProducts_should_return_Products_with_ProductCategory_and_ProductType()
	{
		// Given
		var givenProductCategoryId = await _bookStoreDriver.SeedProductCategory(_fixture
			.Build<CreateProductCategoryCommand>()
			.With(x => x.Subcategories, [])
			.Create());
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.ProductCategoryIds, [givenProductCategoryId])
			.Create();
		var givenBookId = await _bookStoreDriver.SeedBook(givenCreateBookCommand);
		await _bookStoreDriver.SeedBook(givenCreateBookCommand with { ProductCategoryIds = [] });

		// When
		var response = await _bookStoreApi.GetProducts<BookDto>(new GetProductsQuery(ProductCategoryId: givenProductCategoryId, ProductType: ProductType.Book));

		// Then
		response.Should().NotBeNull();
		response.Should().NotBeEmpty();
		response.Count.Should().Be(1);
		response.First().Should().BeEquivalentTo(
			new BookDto
			{
				Id = givenBookId,
				Name = givenCreateBookCommand.Name,
				ProductType = ProductType.Book,
				RetailPrice = givenCreateBookCommand.RetailPrice,
				Price = givenCreateBookCommand.Price,
				DiscountAmmount = givenCreateBookCommand.DiscountAmmount,
				Author = givenCreateBookCommand.Author,
				Description = givenCreateBookCommand.Description,
				CoverImageUrl = givenCreateBookCommand.CoverImageUrl,
				ProductCategoryIds = [givenProductCategoryId]
			});
	}

	[Fact]
	public async Task GetProducts_should_return_empty_collection_when_there_are_no_products()
	{
		// Given
		// When
		var response = await _bookStoreApi.GetProducts<BookDto>();

		// Then
		response.Should().NotBeNull();
		response.Should().BeEmpty();
	}

	#endregion
	
	#region SearchProducts
	
	[Fact]
	public async Task SearchProducts_should_return_matching_Products()
	{
		// Given
		var givenCreateBookCommand1 = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.Name, "Sample Book With Long name")
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenCreateBookCommand2 = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.Name, "Other book")
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenBook1Id = await _bookStoreDriver.SeedBook(givenCreateBookCommand1);
		await _bookStoreDriver.SeedBook(givenCreateBookCommand2);

		// When
		var response = await _bookStoreApi.SearchProducts<BookDto>("sample book with");

		// Then
		response.Should().NotBeNull();
		response.Should().NotBeEmpty();
		response.Count.Should().Be(1);
		response.First().Id.Should().Be(givenBook1Id);
		response.First().Name.Should().Be(givenCreateBookCommand1.Name);
	}
	
	[Fact]
	public async Task SearchProducts_should_return_multiple_matching_Products()
	{
		// Given
		var givenCreateBookCommand1 = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.Name, "Sample Book With Long name")
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenCreateBookCommand2 = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.Name, "Other BOOK")
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenBook1Id = await _bookStoreDriver.SeedBook(givenCreateBookCommand1);
		var givenBook2Id = await _bookStoreDriver.SeedBook(givenCreateBookCommand2);

		// When
		var response = await _bookStoreApi.SearchProducts<BookDto>("book");

		// Then
		response.Should().NotBeNull();
		response.Should().NotBeEmpty();
		response.Count.Should().Be(2);
		response.Should().ContainSingle(x => x.Id == givenBook1Id && x.Name == givenCreateBookCommand1.Name);
		response.Should().ContainSingle(x => x.Id == givenBook2Id && x.Name == givenCreateBookCommand2.Name);
	}
	
	[Fact]
	public async Task SearchProducts_should_return_empty_response_when_search_term_does_not_match_any_products()
	{
		// Given
		await _bookStoreDriver.SeedBook(
			_fixture
				.Build<CreateBookCommand>()
				.With(x => x.Name, "Sample Book With Long name")
				.With(x => x.ProductCategoryIds, [])
				.Create());

		// When
		var response = await _bookStoreApi.SearchProducts<BookDto>("this book does not exist");

		// Then
		response.Should().NotBeNull();
		response.Should().BeEmpty();
		response.Count.Should().Be(0);
	}
	
	#endregion

	#region GetProductById

	[Fact]
	public async Task GetProductById_should_return_Product()
	{
		// Given
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
			.With(x => x.ProductCategoryIds, [])
			.Create();
		var givenBookId = await _bookStoreDriver.SeedBook(givenCreateBookCommand);

		// When
		var response = await _bookStoreApi.GetProductById<BookDto>(givenBookId);

		// Then
		response.Should().NotBeNull();
		response.Should().BeEquivalentTo(
			new BookDto
			{
				Id = givenBookId,
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
	public async Task GetProductById_should_throw_404_NotFound_ApiException_when_product_does_not_exist()
	{
		// Given
		// When
		Func<Task> act = async () => await _bookStoreApi.GetProductById<BookDto>(Guid.NewGuid());

		// Then
		var exception = await act.Should().ThrowAsync<ApiException>();
		exception.Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	#endregion

	#region DeleteProduct

	[Fact]
	public async Task DeleteProduct_should_succeed_for_Admin_user()
	{
		// Given
		AuthenticateAdmin();
		var givenCreateBookCommand = _fixture
			.Build<CreateBookCommand>()
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
}
