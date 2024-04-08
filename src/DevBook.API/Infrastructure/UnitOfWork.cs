namespace DevBook.API.Infrastructure;

/// <summary>
/// Represents database unit of work
/// </summary>
public interface IUnitOfWork
{
	/// <summary>
	/// Changes will not be tracked hence cannot be commited after calling this 
	/// </summary>
	void AsNoTrackingQuery();

	/// <summary>
	/// Commits changes to DB
	/// </summary>
	/// <returns>
	/// The number of state entries written to the database.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// An error is encountered when trying to commit changes and <see cref="AsNoTrackingQuery"/> was called before.
	/// </exception>
	Task<int> CommitAsync(CancellationToken cancellationToken = default);
}

internal sealed class UnitOfWork(DevBookDbContext _devBookDbContext) : IUnitOfWork
{
	public void AsNoTrackingQuery()
	{
		_devBookDbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
	}

	public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
		=> _devBookDbContext.ChangeTracker.QueryTrackingBehavior is QueryTrackingBehavior.NoTracking
			? throw new InvalidOperationException("Cannot call CommitAsync when NoTracking behavior is set")
			: await _devBookDbContext.SaveChangesAsync(cancellationToken);
}
