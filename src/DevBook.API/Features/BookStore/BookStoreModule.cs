using DevBook.API.Features.BookStore.Authors;
using DevBook.API.Features.BookStore.Products;
using DevBook.API.Features.BookStore.Products.Books;
using DevBook.API.Features.BookStore.Products.ProductCategories;

internal sealed class BookStoreModule : IFeatureModule
{
	public IServiceCollection RegisterModule(IServiceCollection services)
	{
		return services;
	}

	public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpointsBuilder)
	{
		endpointsBuilder
			.MapGroup("/bookstore/authors")
			.MapAuthorEndpoints()
			.WithTags($"{nameof(BookStoreModule)}_{nameof(AuthorEndpoints)}")
			.RequireAuthorization(DevBookAccessPolicies.RequireAdmin);

		endpointsBuilder
			.MapGroup("/bookstore/products")
			.MapProductEndpoints()
			.WithTags($"{nameof(BookStoreModule)}_{nameof(ProductEndpoints)}")
			.RequireAuthorization(DevBookAccessPolicies.RequireAdmin);

		endpointsBuilder
			.MapGroup("/bookstore/books")
			.MapBookEndpoints()
			.WithTags($"{nameof(BookStoreModule)}_{nameof(BookEndpoints)}")
			.RequireAuthorization(DevBookAccessPolicies.RequireAdmin);

		endpointsBuilder
			.MapGroup("/bookstore/productCategories")
			.MapProductCategoryEndpoints()
			.WithTags($"{nameof(BookStoreModule)}_{nameof(ProductCategoryEndpoints)}")
			.RequireAuthorization(DevBookAccessPolicies.RequireAdmin);

		return endpointsBuilder;
	}
}
