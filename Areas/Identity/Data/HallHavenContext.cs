using HallHaven.Areas.Identity.Data;
using HallHaven.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace HallHaven.Data;

public class HallHavenContext : IdentityDbContext<HallHavenUser>
{
    public HallHavenContext(DbContextOptions<HallHavenContext> options)
        : base(options)
    {
    }
    public virtual DbSet<CreditHour> CreditHours { get; set; } = null!;
    public virtual DbSet<Dorm> Dorms { get; set; } = null!;
    public virtual DbSet<Form> Forms { get; set; } = null!;
    public virtual DbSet<Gender> Genders { get; set; } = null!;
    public virtual DbSet<Major> Majors { get; set; } = null!;
    public virtual DbSet<Match> Matches { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        modelBuilder.Entity<HallHavenUser>()
        .Property(p => p.DisplayName)
        .HasComputedColumnSql("[FirstName] + ' ' + [LastName]");

        modelBuilder.Entity<CreditHour>(entity =>
        {
            entity.ToTable("CreditHour");

            entity.Property(e => e.Classification)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.CreditHourName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Dorm>(entity =>
        {
            entity.ToTable("Dorm");

            entity.Property(e => e.DormName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.CreditHour)
                .WithMany(p => p.Dorms)
                .HasForeignKey(d => d.CreditHourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Dorm_CreditHour");

            entity.HasOne(d => d.Gender)
                .WithMany(p => p.Dorms)
                .HasForeignKey(d => d.GenderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Form>(entity =>
        {
            entity.ToTable("Form");

            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.CreditHour)
                .WithMany(p => p.Forms)
                .HasForeignKey(d => d.CreditHourId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Dorm)
                .WithMany(p => p.Forms)
                .HasForeignKey(d => d.DormId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Major)
                .WithMany(p => p.Forms)
                .HasForeignKey(d => d.MajorId);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Forms)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.ToTable("Gender");

            entity.Property(e => e.Gender1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Gender");
        });

        modelBuilder.Entity<Major>(entity =>
        {
            entity.ToTable("Major");

            entity.Property(e => e.MajorName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.Property(e => e.ApplicationUser1Id).HasMaxLength(450);

            entity.Property(e => e.ApplicationUser2Id).HasMaxLength(450);

            entity.Property(e => e.User1Id).HasMaxLength(450);

            entity.Property(e => e.User2Id).HasMaxLength(450);

            entity.HasOne(d => d.User1)
                .WithMany(p => p.MatchUser1s)
                .HasForeignKey(d => d.User1Id);

            entity.HasOne(d => d.User2)
                .WithMany(p => p.MatchUser2s)
                .HasForeignKey(d => d.User2Id);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.DisplayName).HasComputedColumnSql("(([FirstName]+' ')+[LastName])", false);

            entity.Property(e => e.Email).HasMaxLength(256);

            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

            entity.HasOne(d => d.Gender)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.GenderId);
        });


        modelBuilder.ApplyConfiguration(new HallHavenUserEntityConfiguration());
    }

    public class HallHavenUserEntityConfiguration : IEntityTypeConfiguration<HallHavenUser>
    {
        public void Configure(EntityTypeBuilder<HallHavenUser> builder)
        {

        }
    }
}
