namespace DevBook.API.Infrastructure.Validation;

/// <summary>
/// Handles validation for both commands and queries
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
internal sealed class ValidationPipelineBehavior<TRequest, TResponse>
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> validators;

	public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators) => this.validators = validators;

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		if (validators.Any())
		{
			var context = new ValidationContext<TRequest>(request);
			var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
			var failures = validationResults.SelectMany(r => r.Errors).Where(failure => failure is not null).ToList();

			if (failures.Count != 0)
			{
				throw new DevBookValidationException(failures);
			}
		}

		return await next();
	}
}
