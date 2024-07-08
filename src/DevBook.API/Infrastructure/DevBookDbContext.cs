using DevBook.API.Features.TimeTracking.Shared;
using DevBook.API.Features.TimeTracking.Tasks;

namespace DevBook.API.Infrastructure;

/// <summary>
/// DevBookDbContext - entities are filtered by userId(owner) when querying and saved with ownerId(user) hence HttpContextAccessor and logged in user is necessity, otherwise throws UnauthorizedAccessException.
/// Identity stores are available without userId to allow authentication.
/// </summary>
/// <param name="options"></param>
/// <param name="httpContextAccessor"></param>
/// <exception cref="UnauthorizedAccessException">Will throw when httpContextAccessor does not contain user.</exception>
public sealed class DevBookDbContext(DbContextOptions<DevBookDbContext> options, IHttpContextAccessor httpContextAccessor) : IdentityDbContext<DevBookUser>(options)
{
	public DbSet<Project> Projects { get; set; }
	public DbSet<WorkTask> Tasks { get; set; }

	private Guid _ownerId => httpContextAccessor.GetUserId();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Project>().Property<Guid>(nameof(_ownerId)).HasColumnName("OwnerId");
		modelBuilder.Entity<WorkTask>().Property<Guid>(nameof(_ownerId)).HasColumnName("OwnerId");

		// Configure entity filters
		modelBuilder.Entity<Project>().HasQueryFilter(x => EF.Property<Guid>(x, nameof(_ownerId)) == _ownerId);
		modelBuilder.Entity<WorkTask>().HasQueryFilter(x => EF.Property<Guid>(x, nameof(_ownerId)) == _ownerId);
	}

	public override int SaveChanges()
	{
		SetChangeTrackerEntitiesOwnerId();

		return base.SaveChanges();
	}

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		SetChangeTrackerEntitiesOwnerId();

		return base.SaveChangesAsync(cancellationToken);
	}

	private void SetChangeTrackerEntitiesOwnerId()
	{
		ChangeTracker.DetectChanges();

		foreach (var item in ChangeTracker.Entries().Where(
					 e =>
						 e.State == EntityState.Added && e.Metadata.GetProperties().Any(p => p.Name == nameof(_ownerId))))
		{
			item.CurrentValues[nameof(_ownerId)] = _ownerId;
		}
	}
}
