using DevBook.API.Helpers;

namespace DevBook.API.Features.BookStore.Authors;

internal sealed record GetAuthorsQuery(int? PageSize, int? ItemLimit) : IQuery<IEnumerable<Author>>;

internal sealed class GetAuthorsQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetAuthorsQuery, IEnumerable<Author>>
{
	public async Task<IEnumerable<Author>> Handle(GetAuthorsQuery query, CancellationToken cancellationToken)
	{
		return await dbContext.Authors
			.OrderBy(x => x.Name)
			.Skip(PagingHelper.NormalizeItemLimit(query.ItemLimit))
			.Take(PagingHelper.NormalizePageSize(query.PageSize))
			.ToListAsync(cancellationToken);
	}
}
