using DevBook.API.Features.BookStore.Authors;
using DevBook.API.Features.BookStore.Books;

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
			.MapGroup("/bookstore/books")
			.MapBookEndpoints()
			.WithTags($"{nameof(BookStoreModule)}_{nameof(BookEndpoints)}")
			.RequireAuthorization(DevBookAccessPolicies.RequireAdmin);

		return endpointsBuilder;
	}
}
