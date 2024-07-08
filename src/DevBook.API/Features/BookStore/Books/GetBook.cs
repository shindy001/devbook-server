namespace DevBook.API.Features.BookStore.Books;

internal sealed record GetBookQuery(Guid Id) : IQuery<OneOf<Book, NotFound>>;

internal class GetBookQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetBookQuery, OneOf<Book, NotFound>>
{
	public async Task<OneOf<Book, NotFound>> Handle(GetBookQuery query, CancellationToken cancellationToken)
	{
		var book = await dbContext.Books.FindAsync([query.Id], cancellationToken);

		return book is null
			? new NotFound()
			: book;
	}
}
