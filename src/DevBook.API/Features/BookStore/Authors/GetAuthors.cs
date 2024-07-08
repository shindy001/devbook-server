namespace DevBook.API.Features.BookStore.Authors;

internal sealed record GetAuthorsQuery(int? PageSize, int? ItemLimit) : IQuery<IEnumerable<Author>>;

internal sealed class GetAuthorsQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetAuthorsQuery, IEnumerable<Author>>
{
	public async Task<IEnumerable<Author>> Handle(GetAuthorsQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.Authors
			.OrderBy(x => x.Name)
			.Skip(request.ItemLimit is null || request.ItemLimit.Value < 0 ? 0 : request.ItemLimit.Value)
			.Take(request.PageSize is null || request.PageSize.Value < 0 ? ApiConstants.MaxPageSize : request.PageSize.Value)
			.ToListAsync(cancellationToken);
	}
}
