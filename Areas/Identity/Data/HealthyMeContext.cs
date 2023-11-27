using HealthyMe.Areas.Identity.Data;
using HealthyMe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace HealthyMe.Areas.Identity.Data;

public class HealthyMeContext : IdentityDbContext<User>
{
    public DbSet<Meal> Meals { get; set; }
    public DbSet<Food> Foods { get; set; }

    public DbSet<MealItem> MealItems { get; set; }

    public HealthyMeContext(DbContextOptions<HealthyMeContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<Meal>()
             .HasOne(m => m.id)
             .WithMany(u => u.Meals)
             .HasForeignKey(m => m.UserId)
             .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Meal>()
        .HasMany(m => m.MealItems)
        .WithOne(mi => mi.Meal)
        .OnDelete(DeleteBehavior.Cascade);

        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());

        builder.Entity<Food>()
        .HasMany(f => f.MealItems)
        .WithOne(mi => mi.Food)
        .OnDelete(DeleteBehavior.Cascade);

    }
}

public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.FirstName).HasMaxLength(50);
        builder.Property(x => x.LastName).HasMaxLength(50);
    }
}
