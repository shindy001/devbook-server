using DevBook.API.Features.BookStore.Products.Books;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace DevBook.API.Features.BookStore.Products;

[SwaggerSubType(typeof(BookDto))]
[JsonDerivedType(typeof(BookDto))]
public abstract record ProductDto()
{
	[Required]
	public required Guid Id { get; init; }

	[Required]
	public required string Name { get; init; }

	[Required]
	public required ProductType ProductType { get; init; }

	[Required]
	public required decimal RetailPrice { get; init; }

	[Required]
	public required decimal Price { get; init; }

	[Required]
	public required decimal DiscountAmmount { get; init; }

	public string? Description { get; init; }
	public string? CoverImageUrl { get; init; }
	public IList<Guid> ProductCategoryIds { get; init; } = [];
}

public static class ProductMappings
{
	public static ProductDto ToDto(this Product product)
	{
		return product switch
		{
			Book book => new BookDto
			{
				Id = product.Id,
				Name = product.Name,
				ProductType = product.ProductType,
				RetailPrice = product.RetailPrice,
				Price = product.Price,
				DiscountAmmount = product.DiscountAmmount,
				Author = book.Author,
				Description = product.Description,
				CoverImageUrl = product.CoverImageUrl,
				ProductCategoryIds = product.ProductCategoryIds,
			},
			_ => throw new InvalidOperationException($"Missing mapping for product type {product.GetType().Name}.")
		};
	}
}
