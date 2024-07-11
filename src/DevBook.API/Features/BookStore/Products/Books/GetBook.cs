namespace DevBook.API.Features.BookStore.Products.Books;

internal sealed record GetBookQuery(Guid Id) : IQuery<OneOf<Book, NotFound>>;

internal class GetBookQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetBookQuery, OneOf<Book, NotFound>>
{
	public async Task<OneOf<Book, NotFound>> Handle(GetBookQuery query, CancellationToken cancellationToken)
	{
		var product = await dbContext.Products.FindAsync([query.Id], cancellationToken);

		return product is null
			? new NotFound()
			: (Book)product;
	}
}
