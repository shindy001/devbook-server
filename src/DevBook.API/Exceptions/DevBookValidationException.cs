namespace DevBook.API.Exceptions;

internal sealed class DevBookValidationException : Exception
{
	public IDictionary<string, string[]> Errors { get; }

	public DevBookValidationException()
		: base("One or more validation failures have occurred.")
	{
		this.Errors = new Dictionary<string, string[]>();
	}

	public DevBookValidationException(string propertyName, string error)
		: base("One or more validation failures have occurred.")
	{
		this.Errors = new Dictionary<string, string[]>
			{
				{ propertyName, new []{ error } }
			};
	}

	public DevBookValidationException(IDictionary<string, string[]> errors)
		: base("One or more validation failures have occurred.")
	{
		this.Errors = errors;
	}

	public DevBookValidationException(IEnumerable<ValidationFailure> failures)
		: this()
	{
		this.Errors = failures
			.GroupBy(e => e.PropertyName, e => e.ErrorMessage)
			.ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
	}
}
