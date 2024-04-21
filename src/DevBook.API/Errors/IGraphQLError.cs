namespace DevBook.API.Errors;

internal interface IGraphQLError
{
	string Message { get; }
}
