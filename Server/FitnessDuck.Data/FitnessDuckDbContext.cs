using FitnessDuck.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessDuck.Data;

//export DB_PROVIDER=postgres
//dotnet ef migrations add pg_InitialCreate --output-dir Migrations/Postgres --project ./FitnessDuck.Data --startup-project ./FitnessDuck.Api

//export DB_PROVIDER=sqlite
//dotnet ef migrations add sqllite_InitialCreate --output-dir Migrations/SQLite --project ./FitnessDuck.Data --startup-project ./FitnessDuck.Api


public class FitnessDuckDbContext:DbContext

{
    public DbSet<ScheduleEntity> Schedules { get; set; } = default!;
    public DbSet<LessonEntity> Lessons { get; set; } = default!;
    public DbSet<UserEntity> Users { get; set; } = default!;
    public DbSet<BookingEntity> Bookings { get; set; } = default!;
    public DbSet<OtpCacheEntity> TempOtps { get; set; } = default!;
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; } = default!;

    public FitnessDuckDbContext(DbContextOptions<FitnessDuckDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Schedule
        modelBuilder.Entity<ScheduleEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.HasMany(e => e.Lessons)
                  .WithOne(l => l.Schedule)
                  .HasForeignKey(l => l.ScheduleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Lesson
        modelBuilder.Entity<LessonEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Date).IsRequired();
            entity.HasMany(e => e.Bookings)
                  .WithOne(b => b.Lesson)
                  .HasForeignKey(b => b.LessonId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // User
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Role).IsRequired();
        });

        // Booking
        modelBuilder.Entity<BookingEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(b => b.User)
                  .WithMany(u => u.Bookings)
                  .HasForeignKey(b => b.UserId);
            entity.HasOne(b => b.Lesson)
                  .WithMany(l => l.Bookings)
                  .HasForeignKey(b => b.LessonId);
        });

        // OTP
        modelBuilder.Entity<OtpCacheEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Contact).IsRequired();
            entity.Property(e => e.Method).IsRequired();
        });

        // Refresh Tokens
        modelBuilder.Entity<RefreshTokenEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
        });

    }
}











