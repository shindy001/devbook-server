using DevBook.API.Helpers;

namespace DevBook.API.Features.BookStore.Books;

internal sealed record GetBooksQuery(int? PageSize, int? ItemLimit) : IQuery<IEnumerable<Book>>;

internal sealed class GetBooksQueryHandler(DevBookDbContext dbContext) : IQueryHandler<GetBooksQuery, IEnumerable<Book>>
{
	public async Task<IEnumerable<Book>> Handle(GetBooksQuery query, CancellationToken cancellationToken)
	{
		return await dbContext.Products
			.OrderBy(x => x.Name)
			.Skip(PagingHelper.NormalizeItemLimit(query.ItemLimit))
			.Take(PagingHelper.NormalizePageSize(query.PageSize))
			.Cast<Book>()
			.ToListAsync(cancellationToken);
	}
}
