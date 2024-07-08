using DevBook.API.Helpers;

namespace DevBook.API.Features.BookStore.Authors;

internal sealed record GetAuthorsQuery(int? PageSize, int? ItemLimit) : IQuery<IEnumerable<Author>>;

internal sealed class GetAuthorsQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetAuthorsQuery, IEnumerable<Author>>
{
	public async Task<IEnumerable<Author>> Handle(GetAuthorsQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.Authors
			.OrderBy(x => x.Name)
			.Skip(PagingHelper.NormalizeItemLimit(request.ItemLimit))
			.Take(PagingHelper.NormalizePageSize(request.PageSize))
			.ToListAsync(cancellationToken);
	}
}
