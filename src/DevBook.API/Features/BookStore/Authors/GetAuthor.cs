namespace DevBook.API.Features.BookStore.Authors;

public sealed record GetAuthorQuery(Guid Id) : IQuery<OneOf<Author, NotFound>>;

internal class GetAuthorQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetAuthorQuery, OneOf<Author, NotFound>>
{
	public async Task<OneOf<Author, NotFound>> Handle(GetAuthorQuery query, CancellationToken cancellationToken)
	{
		var author = await dbContext.Authors.FindAsync([query.Id], cancellationToken);

		return author is null
			? new NotFound()
			: author;
	}
}
