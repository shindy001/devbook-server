using DevBook.API.Features.BookStore;
using DevBook.API.Features.BookStore.ProductCategories;
using DevBook.API.Features.BookStore.Products;
using DevBook.API.Features.BookStore.Products.Books;

internal sealed class BookStoreModule : IFeatureModule
{

	public IServiceCollection RegisterModule(IServiceCollection services)
	{
		services.AddSingleton<BookStoreDataSeeder>();
		return services;
	}

	public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpointsBuilder)
	{
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

	public async Task InitializeModule(AsyncServiceScope serviceScope)
	{
		var seeder = serviceScope.ServiceProvider.GetService<BookStoreDataSeeder>();
		if (seeder != null)
		{
			var dbContext = serviceScope.ServiceProvider.GetRequiredService<DevBookDbContext>();
			if (!dbContext.Products.Any() && !dbContext.ProductCategories.Any())
			{
				var categories = await seeder.SeedCategories(dbContext);
				await seeder.SeedBooks(dbContext, categories);
				await dbContext.SaveChangesAsync();
			}
		}
	}
}
