namespace DevBook.API.Helpers;

internal static class PagingHelper
{
	public static int NormalizePageSize(int? pageSize)
	{
		return pageSize is null || pageSize.Value < 0
			? ApiConstants.MaxPageSize
			: pageSize.Value;
	}

	public static int NormalizeItemLimit(int? itemLimit)
	{
		return itemLimit is null || itemLimit.Value < 0
			? 0
			: itemLimit.Value;
	}
}
