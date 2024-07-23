using DevBook.API.Features.BookStore.Authors;
using DevBook.API.Features.BookStore.ProductCategories;
using DevBook.API.Features.BookStore.Products;
using DevBook.API.Features.BookStore.Products.Books;

namespace DevBook.API.IntegrationTests.Api;

public interface IBookStoreApi
{
	#region Authors

	[Get("/bookstore/authors")]
	Task<IList<AuthorDto>> GetAuthors();

	[Get("/bookstore/authors/{id}")]
	Task<AuthorDto> GetAuthorById(Guid id);

	[Post("/bookstore/authors")]
	Task<HttpResponseMessage> CreateAuthor(CreateAuthorCommand command);

	[Put("/bookstore/authors/{id}")]
	Task<HttpResponseMessage> UpdateAuthor(Guid id, UpdateAuthorCommandDto command);

	[Patch("/bookstore/authors/{id}")]
	Task<HttpResponseMessage> PatchAuthor(Guid id, PatchAuthorCommandDto command);

	[Delete("/bookstore/authors/{id}")]
	Task<HttpResponseMessage> DeleteAuthor(Guid id);

	#endregion

	#region Products

	[Get("/bookstore/products")]
	Task<IList<T>> GetProducts<T>(GetProductsQuery? query = null);

	[Get("/bookstore/products/{id}")]
	Task<T> GetProductById<T>(Guid id);

	[Delete("/bookstore/products/{id}")]
	Task<HttpResponseMessage> DeleteProduct(Guid id);

	#endregion

	#region ProductCategories

	[Get("/bookstore/productCategories")]
	Task<IList<ProductCategoryDto>> GetProductCategories();

	[Get("/bookstore/productCategories/{id}")]
	Task<ProductCategoryDto> GetProductCategoryById(Guid id);

	[Post("/bookstore/productCategories")]
	Task<HttpResponseMessage> CreateProductCategory(CreateProductCategoryCommand command);

	[Put("/bookstore/productCategories/{id}")]
	Task<HttpResponseMessage> UpdateProductCategory(Guid id, UpdateProductCategoryCommandDto command);

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
