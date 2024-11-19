namespace DevBook.API.Helpers;

internal static class PagingHelper
{
	public static int NormalizePageSize(int? pageSize)
	{
		return pageSize is null or < 0
			? ApiConstants.MaxPageSize
			: pageSize.Value;
	}

	public static int NormalizeOffset(int? offset)
	{
		return offset is null or < 0
			? 0
			: offset.Value;
	}
}
