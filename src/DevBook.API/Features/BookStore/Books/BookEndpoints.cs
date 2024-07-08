namespace DevBook.API.Features.BookStore.Books;

internal static class BookEndpoints
{
	private const string OperationIdPrefix = "Books.";
	private const string GetByIdRoute = "GetById";

	public static RouteGroupBuilder MapBookEndpoints(this RouteGroupBuilder groupBuilder)
	{
		groupBuilder.MapGet("/", GetBooks)
			.WithName($"{OperationIdPrefix}GetAll")
			.Produces<IList<Book>>()
			.AllowAnonymous();

		groupBuilder.MapPost("/", CreateBook)
			.WithName($"{OperationIdPrefix}Create")
			.Produces(StatusCodes.Status201Created);

		groupBuilder.MapGet("/{id:guid}", GetBookById)
			.WithName($"{OperationIdPrefix}{GetByIdRoute}")
			.Produces<Book>()
			.Produces(StatusCodes.Status404NotFound)
			.AllowAnonymous();

		groupBuilder.MapPut("/{id:guid}", UpdateBook)
			.WithName($"{OperationIdPrefix}Update")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapPatch("/{id:guid}", PatchBook)
			.WithName($"{OperationIdPrefix}Patch")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapDelete("/{id:guid}", DeleteBook)
			.WithName($"{OperationIdPrefix}Delete")
			.Produces(StatusCodes.Status204NoContent);

		return groupBuilder;
	}

	private static async Task<IResult> GetBooks(IExecutor executor, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	private static async Task<IResult> CreateBook(/*CreateBookCommand command,*/ IExecutor executor, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();

	}

	private static async Task<IResult> GetBookById(Guid id, /* IExecutor executor,*/ CancellationToken cancellationToken)
	{
		throw new NotImplementedException();

	}

	private static async Task<IResult> UpdateBook([FromRoute] Guid id, /*UpdateBookCommandDto command,*/ IExecutor executor, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();

	}

	private static async Task<IResult> PatchBook([FromRoute] Guid id, /*PatchBookCommandDto command,*/ IExecutor executor, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();

	}

	private static async Task<IResult> DeleteBook([FromRoute] Guid id, IExecutor executor, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();

	}
}
