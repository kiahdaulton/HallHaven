using HallHaven.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HallHaven.Data;

public class HallHavenContext : IdentityDbContext<HallHavenUser>
{
    public HallHavenContext(DbContextOptions<HallHavenContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<HallHavenUser>()
        .Property(p => p.DisplayName)
        .HasComputedColumnSql("[FirstName] + ' ' + [LastName]");


        builder.ApplyConfiguration(new HallHavenUserEntityConfiguration());
    }

    public class HallHavenUserEntityConfiguration : IEntityTypeConfiguration<HallHavenUser>
    {
        public void Configure(EntityTypeBuilder<HallHavenUser> builder)
        {

        }
    }
}
