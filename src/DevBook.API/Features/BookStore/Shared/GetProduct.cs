namespace DevBook.API.Features.BookStore.Shared;

internal sealed record GetProductQuery(Guid Id) : IQuery<OneOf<Product, NotFound>>;

internal class GetProductQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetProductQuery, OneOf<Product, NotFound>>
{
	public async Task<OneOf<Product, NotFound>> Handle(GetProductQuery query, CancellationToken cancellationToken)
	{
		var product = await dbContext.Products.FindAsync([query.Id], cancellationToken);

		return product is null
			? new NotFound()
			: product;
	}
}
