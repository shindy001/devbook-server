namespace DevBook.API.Features.TimeTracking.Projects;

public sealed record CreateProjectCommand : ICommand<Guid>
{
	[Required]
	public required string Name { get; init; }
	public string? Details { get; init; }
	public int? HourlyRate { get; init; }
	public string? Currency { get; init; }
	public string? HexColor { get; init; }
}

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
	public CreateProjectCommandValidator()
	{
		RuleFor(x => x.Name).NotEmpty();
	}
}

internal sealed class CreateProjectCommandHandler(DevBookDbContext dbContext) : ICommandHandler<CreateProjectCommand, Guid>
{
	public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
	{
		var newItem = new Project { Name = request.Name, Details = request.Details, HourlyRate = request.HourlyRate, Currency = request.Currency, HexColor = request.HexColor };
		await dbContext.Projects.AddAsync(newItem, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
		return newItem.Id;
	}
}
