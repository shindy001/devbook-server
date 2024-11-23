namespace DevBook.API.Middleware;

/// <summary>
/// Handles Internal server error + BadRequest(ValidationError)
/// </summary>
internal sealed class GlobalExceptionHandlerMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IHostEnvironment _env;
	private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

	public GlobalExceptionHandlerMiddleware(
		RequestDelegate next,
		IHostEnvironment env,
		ILogger<GlobalExceptionHandlerMiddleware> logger)
	{
		this._next = next;
		this._env = env;
		this._logger = logger;
	}

	public async Task InvokeAsync(HttpContext httpContext)
	{
		try
		{
			await _next(httpContext);
		}
		catch (DevBookValidationException ex)
		{
			await HandleValidationException(httpContext, ex);
		}
		catch (Exception ex)
		{
			await HandleException(httpContext, ex);
		}
	}

	private async Task HandleException(HttpContext context, Exception ex)
	{
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

		var details = new ProblemDetails
		{
			Status = StatusCodes.Status500InternalServerError,
			Title = "An error occurred while processing your request.",
			Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
			Detail = _env.IsDevelopment() ? ex.ToString() : string.Empty
		};

		var jsonOptions = context.RequestServices.GetService(typeof(IOptions<JsonOptions>)) as IOptions<JsonOptions>;

		await context.Response.WriteAsync(JsonSerializer.Serialize(details, jsonOptions?.Value?.JsonSerializerOptions));
	}

	private async Task HandleValidationException(HttpContext context, DevBookValidationException validationException)
	{
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

		_logger.LogDebug("API {RequestMethod} {RequestPath} Validation errors: {Errors}",
			context.Request.Method,
			context.Request.Path,
			validationException.Errors);

		var details = new ValidationProblemDetails(validationException.Errors)
		{
			Status = StatusCodes.Status400BadRequest,
			Title = "One or more validation failures have occurred.",
			Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
		};

		var jsonOptions = context.RequestServices.GetService(typeof(IOptions<JsonOptions>)) as IOptions<JsonOptions>;

		await context.Response.WriteAsync(JsonSerializer.Serialize(details, jsonOptions?.Value?.JsonSerializerOptions));
	}
}
