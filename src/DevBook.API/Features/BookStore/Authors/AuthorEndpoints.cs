﻿namespace DevBook.API.Features.BookStore.Authors;

internal static class AuthorEndpoints
{
	private const string OperationIdPrefix = "Authors.";

	public static RouteGroupBuilder MapAuthorEndpoints(this RouteGroupBuilder groupBuilder)
	{
		groupBuilder.MapGet("/", GetAuthors)
			.WithName($"{OperationIdPrefix}GetAll")
			.Produces<IList<AuthorDto>>()
			.AllowAnonymous();

		groupBuilder.MapGet("/{id:guid}", GetAuthorById)
			.WithName($"{OperationIdPrefix}{ApiConstants.GetByIdRoute}")
			.Produces<AuthorDto>()
			.Produces(StatusCodes.Status404NotFound)
			.AllowAnonymous();

		groupBuilder.MapPost("/", CreateAuthor)
			.WithName($"{OperationIdPrefix}Create")
			.Produces(StatusCodes.Status201Created);

		groupBuilder.MapPut("/{id:guid}", UpdateAuthor)
			.WithName($"{OperationIdPrefix}Update")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapPatch("/{id:guid}", PatchAuthor)
			.WithName($"{OperationIdPrefix}Patch")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

		groupBuilder.MapDelete("/{id:guid}", DeleteAuthor)
			.WithName($"{OperationIdPrefix}Delete")
			.Produces(StatusCodes.Status204NoContent);

		return groupBuilder;
	}

	private static async Task<IResult> GetAuthors([AsParameters] GetAuthorsQuery query, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(query, cancellationToken);
		var dtos = result.Select(x => x.ToDto());
		return TypedResults.Ok(dtos);
	}

	private static async Task<IResult> GetAuthorById([AsParameters] GetAuthorQuery query, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteQuery(query, cancellationToken);
		return result.Match<IResult>(
			author => TypedResults.Ok(author.ToDto()),
			notFound => TypedResults.NotFound(query.Id));
	}

	private static async Task<IResult> CreateAuthor(CreateAuthorCommand command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(command, cancellationToken);
		return TypedResults.CreatedAtRoute($"{OperationIdPrefix}{ApiConstants.GetByIdRoute}", new { id = result.Id });
	}

	private static async Task<IResult> UpdateAuthor([FromRoute] Guid id, UpdateAuthorCommandDto command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(
			new UpdateAuthorCommand(
				Id: id,
				Name: command.Name,
				Description: command.Description),
			cancellationToken);

		return result.Match<IResult>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> PatchAuthor([FromRoute] Guid id, PatchAuthorCommandDto command, IExecutor executor, CancellationToken cancellationToken)
	{
		var result = await executor.ExecuteCommand(
			new PatchAuthorCommand(
				Id: id,
				Name: command.Name,
				Description: command.Description),
		cancellationToken);

		return result.Match<IResult>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound(id));
	}

	private static async Task<IResult> DeleteAuthor([AsParameters] DeleteAuthorCommand command, IExecutor executor, CancellationToken cancellationToken)
	{
		await executor.ExecuteCommand(command, cancellationToken);
		return TypedResults.NoContent();
	}
}
