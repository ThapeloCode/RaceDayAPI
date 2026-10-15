using Microsoft.EntityFrameworkCore;
using RaceDay.Models;
using Route = RaceDay.Models.Route;

namespace RaceDay.Data;

public class RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Route> Routes => Set<Route>();
    public DbSet<Registration> Registrations => Set<Registration>();
    public DbSet<Result> Results => Set<Result>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .HasOne(c => c.Event)
            .WithMany(e => e.Categories)
            .HasForeignKey(c => c.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Route>()
            .HasOne(r => r.Category)
            .WithOne(c => c.Route)
            .HasForeignKey<Route>(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Registration>()
            .HasOne(r => r.User)
            .WithMany(u => u.Registrations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Registration>()
            .HasOne(r => r.Category)
            .WithMany(c => c.Registrations)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Result>()
            .HasOne(res => res.Registration)
            .WithOne(reg => reg.Result)
            .HasForeignKey<Result>(res => res.RegistrationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}