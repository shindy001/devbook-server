using DevBook.API.Features.BookStore.Authors;
using DevBook.API.Features.BookStore.ProductCategories;
using DevBook.API.Features.BookStore.Products;
using DevBook.API.Features.BookStore.Products.Books;
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
	#region TimeTracking module

	public DbSet<Project> Projects { get; set; }
	public DbSet<WorkTask> Tasks { get; set; }

	#endregion

	#region BookStore module

	public DbSet<Author> Authors { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<ProductCategory> ProductCategories { get; set; }

	#endregion

	private Guid OwnerId => httpContextAccessor.GetUserId();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		#region per user data

		modelBuilder.Entity<Project>().Property<Guid>(nameof(OwnerId)).HasColumnName("OwnerId");
		modelBuilder.Entity<WorkTask>().Property<Guid>(nameof(OwnerId)).HasColumnName("OwnerId");

		// Configure entity filters
		modelBuilder.Entity<Project>().HasQueryFilter(x => EF.Property<Guid>(x, nameof(OwnerId)) == OwnerId);
		modelBuilder.Entity<WorkTask>().HasQueryFilter(x => EF.Property<Guid>(x, nameof(OwnerId)) == OwnerId);

		#endregion

		#region BookStore module

		modelBuilder.Entity<Product>()
			.HasDiscriminator<ProductType>(nameof(ProductType))
			.HasValue<Book>(ProductType.Book);

		#endregion
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
			e => e.State == EntityState.Added && e.Metadata.GetProperties().Any(p => p.Name == nameof(OwnerId))))
		{
			item.CurrentValues[nameof(OwnerId)] = OwnerId;
		}
	}
}
