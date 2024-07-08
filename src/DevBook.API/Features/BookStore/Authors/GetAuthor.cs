namespace DevBook.API.Features.BookStore.Authors;

internal sealed record GetAuthorQuery(Guid Id) : IQuery<OneOf<Author, NotFound>>;

internal class GetAuthorQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetAuthorQuery, OneOf<Author, NotFound>>
{
	public async Task<OneOf<Author, NotFound>> Handle(GetAuthorQuery request, CancellationToken cancellationToken)
	{
		var author = await dbContext.Authors.FindAsync([request.Id], cancellationToken);

		return author is null
			? new NotFound()
			: author;
	}
}
