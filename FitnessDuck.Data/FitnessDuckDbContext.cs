using System.Text.Json;
using FitnessDuck.Data.Entities;
using FitnessDuck.Models.Common;
using FitnessDuck.Models.Entities;
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
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; } = default!;
    public DbSet<ActivityTypeEntity> ActivityTypes { get; set; } = default!;
    public DbSet<AuthTokenEntity> AuthTokens { get; set; } = default!;
    public DbSet<NotificationsOutboxEntity> Outbox { get; set; } = default!;

    
    
    
    public FitnessDuckDbContext(DbContextOptions<FitnessDuckDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var JsonOption = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        
        // Schedule
        modelBuilder.Entity<ScheduleEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.StartDateUtc).IsRequired();
            entity.Property(e => e.WeekPlan).IsRequired();
            entity.Property(e => e.Recurrence).IsRequired();

            entity.HasMany(e => e.Lessons)
                  .WithOne(l => l.Schedule)
                  .HasForeignKey(l => l.ScheduleId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            
            
            entity.Property(e => e.WeekPlan).HasConversion(
                    v => JsonSerializer.Serialize(v, JsonOption),
                    v => JsonSerializer.Deserialize<List<DayPlan>>(v, JsonOption) ?? DayPlan.BuildWeekPlan())
                .HasColumnType(Database.IsNpgsql() ? "jsonb" : "TEXT");
        });

        // Lesson
        modelBuilder.Entity<LessonEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StartDateUtc).IsRequired();
            entity.Property(e => e.EndDateUtc).IsRequired();
            entity.HasMany(e => e.Bookings)
                  .WithOne(b => b.Lesson)
                  .HasForeignKey(b => b.LessonId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(b => b.Schedule)
                .WithMany(l => l.Lessons)
                .HasForeignKey(b => b.ScheduleId);
        });

        // User
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.TelegramChatId).IsUnique(false);
            
            entity.Property(e => e.Role).IsRequired();
            entity.Property(e => e.Local).IsRequired();
            //entity.Property(e => e.Name).IsRequired();
            //entity.Property(e => e.Surname).IsRequired();
        });
        
        
        modelBuilder.Entity<AuthTokenEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(t => new { t.Contact, t.Token }).IsUnique();


        });
        
               
        // Activity Type
        modelBuilder.Entity<ActivityTypeEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
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
            
            entity.Property(e => e.Status).HasConversion<string>();
        });

       

        // Refresh Tokens
        modelBuilder.Entity<RefreshTokenEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            
            entity.HasOne(b => b.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(b => b.UserId);
        });
        
        modelBuilder.Entity<NotificationsOutboxEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.Method).IsRequired();
            
            entity.HasOne(b => b.User)
                .WithMany(u => u.PendingMessages)
                .HasForeignKey(b => b.UserId);
        });

    }
}











