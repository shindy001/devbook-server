using DevBook.API.Features.BookStore.Authors;
using DevBook.API.Features.BookStore.Products;
using DevBook.API.Features.BookStore.Products.Books;
using System.Net;

namespace DevBook.API.IntegrationTests.Api;

public interface IBookStoreApi
{
	#region Products

	[Get("/bookstore/products")]
	Task<IList<T>> GetProducts<T>();

	[Get("/bookstore/products/{id}")]
	Task<Product> GetProductById(Guid id);

	[Delete("/bookstore/products/{id}")]
	Task DeleteProduct(Guid id);

	#endregion

	#region Authors

	[Post("/bookstore/authors")]
	Task<HttpResponseMessage> CreateAuthor(CreateAuthorCommand command);

	#endregion

	#region Books

	[Post("/bookstore/books")]
	Task<HttpResponseMessage> CreateBook(CreateBookCommand command);

	#endregion
}

internal static class BookStoreApiExtensions
{
	public static async Task<Guid> SeedAuthor(this IBookStoreApi bookStoreApi, CreateAuthorCommand createCommand)
	{
		var response = await bookStoreApi.CreateAuthor(createCommand);
		return ExtractCreatedLocationGuid(response);
	}

	public static async Task<Guid> SeedBook(this IBookStoreApi bookStoreApi, CreateBookCommand createCommand)
	{
		var response = await bookStoreApi.CreateBook(createCommand);
		return ExtractCreatedLocationGuid(response);
	}

	private static Guid ExtractCreatedLocationGuid(HttpResponseMessage response)
	{
		response.StatusCode.Should().Be(HttpStatusCode.Created);
		var id = response.Headers.Location?.Segments[^1];
		return Guid.Parse(id!);
	}
}
