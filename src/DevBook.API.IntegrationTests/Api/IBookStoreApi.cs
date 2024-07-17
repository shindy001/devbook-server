using DevBook.API.Features.BookStore.Authors;
using DevBook.API.Features.BookStore.ProductCategories;
using DevBook.API.Features.BookStore.Products.Books;

namespace DevBook.API.IntegrationTests.Api;

public interface IBookStoreApi
{
	#region Authors

	[Post("/bookstore/authors")]
	Task<HttpResponseMessage> CreateAuthor(CreateAuthorCommand command);

	#endregion

	#region Products

	[Get("/bookstore/products")]
	Task<IList<T>> GetProducts<T>();

	[Get("/bookstore/products/{id}")]
	Task<T> GetProductById<T>(Guid id);

	[Delete("/bookstore/products/{id}")]
	Task<HttpResponseMessage> DeleteProduct(Guid id);

	#endregion

	#region ProductCategories

	[Get("/bookstore/productCategories")]
	Task<IList<ProductCategory>> GetProductCategories();

	[Get("/bookstore/productCategories/{id}")]
	Task<ProductCategory> GetProductCategoryById(Guid id);

	[Post("/bookstore/productCategories")]
	Task<HttpResponseMessage> CreateProductCategory(CreateProductCategoryCommand command);

	[Put("/bookstore/productCategories/{id}")]
	Task<HttpResponseMessage> UpdateProductCategory(Guid id, UpdateProductCategoryCommand command);

	[Delete("/bookstore/productCategories/{id}")]
	Task<HttpResponseMessage> DeleteProductCategory(Guid id);

	#endregion

	#region Books

	[Post("/bookstore/books")]
	Task<HttpResponseMessage> CreateBook(CreateBookCommand command);

	[Put("/bookstore/books/{id}")]
	Task<HttpResponseMessage> UpdateBook(Guid id, UpdateBookCommandDto command);

	[Patch("/bookstore/books/{id}")]
	Task<HttpResponseMessage> PatchBook(Guid id, PatchBookCommandDto command);

	#endregion
}
