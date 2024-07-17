namespace DevBook.API.IntegrationTests.Extensions;
public static class HttpResponseMessageExtensions
{
	public static Guid? GetGuidFromResponseLocation(this HttpResponseMessage response)
	{
		var success = Guid.TryParse(response.Headers.Location?.Segments[^1], out var result);
		return success ? result : null;
	}
}
