using DevBook.API.Features.BookStore.Products.Books;
using Swashbuckle.AspNetCore.Annotations;

namespace DevBook.API.Features.BookStore.Shared;

[SwaggerDiscriminator(nameof(ProductType))]
[SwaggerSubType(typeof(Book), DiscriminatorValue = nameof(Book))]
public abstract record Product()
	: Entity(Guid.NewGuid())
{
	public required string Name { get; init; }
	public required ProductType ProductType { get; init; }

	public required decimal RetailPrice { get; init; }
	public required decimal Price { get; init; }
	public required decimal DiscountAmmount { get; init; }

	public string? Description { get; init; }
	public string? CoverImageUrl { get; init; }
	public IList<Guid> ProductCategoryIds { get; init; } = [];
}
