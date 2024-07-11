using DevBook.API.Helpers;

namespace DevBook.API.Features.BookStore.Authors;

internal sealed record GetAuthorsQuery(int? PageSize, int? Offset) : IQuery<IEnumerable<Author>>;

internal sealed class GetAuthorsQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetAuthorsQuery, IEnumerable<Author>>
{
	public async Task<IEnumerable<Author>> Handle(GetAuthorsQuery query, CancellationToken cancellationToken)
	{
		return await dbContext.Authors
			.OrderBy(x => x.Name)
			.Skip(PagingHelper.NormalizeOffset(query.Offset))
			.Take(PagingHelper.NormalizePageSize(query.PageSize))
			.ToListAsync(cancellationToken);
	}
}
