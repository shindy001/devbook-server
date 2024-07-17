using DevBook.API.Features.BookStore.Authors;
using DevBook.API.Features.BookStore.ProductCategories;
using DevBook.API.Features.BookStore.Products.Books;
using Microsoft.AspNetCore.Authentication;
using System.Net;

namespace DevBook.API.IntegrationTests.Api;

/// <summary>
/// Driver that executes actions on BookStoreApi.
/// Requires created IBookStoreApi client.
/// </summary>
internal sealed class BookStoreApiDriver
{
	private readonly IBookStoreApi _bookStoreApi;
	private readonly TestAuthInterceptor _testAuthInterceptor;

	public BookStoreApiDriver(IBookStoreApi bookStoreApi, TestAuthInterceptor testAuthInterceptor)
	{
		_bookStoreApi = bookStoreApi;
		_testAuthInterceptor = testAuthInterceptor;
	}

	public async Task<Guid> SeedAuthor(CreateAuthorCommand createCommand)
	{
		return await ExecuteInAdminAuthContext(async () =>
		{
			var response = await _bookStoreApi.CreateAuthor(createCommand);
			return ExtractCreatedLocationGuid(response);
		});
	}

	public async Task<Guid> SeedProductCategory(CreateProductCategoryCommand createCommand)
	{
		return await ExecuteInAdminAuthContext(async () =>
		{
			var response = await _bookStoreApi.CreateProductCategory(createCommand);
			return ExtractCreatedLocationGuid(response);
		});
	}

	public async Task<Guid> SeedBook(CreateBookCommand createCommand)
	{
		return await ExecuteInAdminAuthContext(async () =>
		{
			var response = await _bookStoreApi.CreateBook(createCommand);
			return ExtractCreatedLocationGuid(response);
		});
	}

	private async Task<Guid> ExecuteInAdminAuthContext(Func<Task<Guid>> action)
	{
		var currentAuthentication = _testAuthInterceptor.OnAuthenticate?.Clone() as Func<Task<AuthenticateResult>>;
		_testAuthInterceptor.AuthenticateAdmin();

		var result = await action();

		_testAuthInterceptor.OnAuthenticate = currentAuthentication;

		return result;
	}

	private async Task ExecuteInAdminAuthContext(Func<Task<Action>> action)
	{
		var currentAuthentication = _testAuthInterceptor.OnAuthenticate?.Clone() as Func<Task<AuthenticateResult>>;
		_testAuthInterceptor.AuthenticateAdmin();

		await action();

		_testAuthInterceptor.OnAuthenticate = currentAuthentication;
	}

	private Guid ExtractCreatedLocationGuid(HttpResponseMessage response)
	{
		response.StatusCode.Should().Be(HttpStatusCode.Created);
		var id = response.Headers.Location?.Segments[^1];
		return Guid.Parse(id!);
	}
}
