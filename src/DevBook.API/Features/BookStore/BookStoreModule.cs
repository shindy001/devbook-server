using Bogus;
using DevBook.API.Features.BookStore.ProductCategories;
using DevBook.API.Features.BookStore.Products;
using DevBook.API.Features.BookStore.Products.Books;

internal sealed class BookStoreModule : IFeatureModule
{
	private readonly Random random = new();

	public IServiceCollection RegisterModule(IServiceCollection services)
	{
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
		var dbContext = serviceScope.ServiceProvider.GetRequiredService<DevBookDbContext>();
		if (!dbContext.Products.Any() && !dbContext.ProductCategories.Any())
		{
			var categories = await SeedCategories(dbContext);
			await SeedBooks(dbContext, categories);
			await dbContext.SaveChangesAsync();
		}

	}

	private async Task<ProductCategory[]> SeedCategories(DevBookDbContext dbContext)
	{
		//Set the randomizer seed to generate repeatable data sets.
		Randomizer.Seed = random;

		ProductCategory[] subcategories = [
			new ProductCategory
			{
				Name = "Sci-fi",
				IsTopLevelCategory = false,
				Subcategories = [],
			},
			new ProductCategory
			{
				Name = "Romance",
				IsTopLevelCategory = false,
				Subcategories = [],
			},
		];

		List<ProductCategory> topLevelCategories = [
			new ProductCategory
			{
				Name = "Books",
				IsTopLevelCategory = true,
				Subcategories = subcategories.Select(x => x.Id).ToList(),
			},
			new ProductCategory
			{
				Name = "Ebooks",
				IsTopLevelCategory = true,
				Subcategories = [subcategories.First().Id],
			}
		];

		await dbContext.AddRangeAsync(subcategories);
		await dbContext.AddRangeAsync(topLevelCategories);

		return [.. subcategories, .. topLevelCategories];
	}

	private async Task SeedBooks(DevBookDbContext dbContext, ProductCategory[] productCategories)
	{
		//Set the randomizer seed to generate repeatable data sets.
		Randomizer.Seed = random;

		var books = new Faker<Book>()
			.RuleFor(x => x.Name, f => f.Lorem.Sentence())
			.RuleFor(x => x.ProductType, ProductType.Book)
			.RuleFor(x => x.RetailPrice, f => f.Random.Number(1, 100))
			.RuleFor(x => x.Price, f => f.Random.Number(50, 150))
			.RuleFor(x => x.DiscountAmmount, f => f.Random.Number(0, 1))

			.RuleFor(x => x.Author, f => f.Name.FullName())
			.RuleFor(x => x.Description, f => f.Random.Words())
			.RuleFor(x => x.ProductCategoryIds, f => productCategories
				.Skip(f.Random.Number(0, productCategories.Length - 1))
				.Take(f.Random.Number(0, productCategories.Length - 1))
				.Select(x => x.Id)
				.ToList())
			.Generate(20);

		await dbContext.AddRangeAsync(books);
	}
}
