using DevBook.API.Features.BookStore.Authors;
using DevBook.API.Features.BookStore.Products.Books;

namespace DevBook.API.IntegrationTests.Api;

public interface IBookStoreApi
{
	#region Products

	[Get("/bookstore/products")]
	Task<IList<T>> GetProducts<T>();

	[Get("/bookstore/products/{id}")]
	Task<T> GetProductById<T>(Guid id);

	[Delete("/bookstore/products/{id}")]
	Task<HttpResponseMessage> DeleteProduct(Guid id);

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
