namespace DevBook.API.ReturnTypes;

public sealed record SuccessResult
{
	public string Message => "Operation was successful.";
}
