using Bogus;
using DevBook.API.Features.BookStore.ProductCategories;
using DevBook.API.Features.BookStore.Products;
using DevBook.API.Features.BookStore.Products.Books;

namespace DevBook.API.Features.BookStore;

internal sealed class BookStoreDataSeeder
{
	private readonly Random random = new();

	public async Task<ProductCategory[]> SeedCategories(DevBookDbContext dbContext)
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

	public async Task SeedBooks(DevBookDbContext dbContext, ProductCategory[] productCategories)
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
