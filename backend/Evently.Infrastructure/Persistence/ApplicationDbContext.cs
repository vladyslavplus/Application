using Evently.Domain.Entities;
using Evently.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Evently.Infrastructure.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
    {
        public DbSet<Event> Events => Set<Event>();
        public DbSet<EventParticipant> EventParticipants => Set<EventParticipant>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(2000);

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasIndex(e => e.OrganizerId);
                entity.HasIndex(e => e.StartDate);
                entity.HasIndex(e => e.IsPublic);

                entity.HasOne<ApplicationUser>()
                    .WithMany(u => u.OrganizedEvents)
                    .HasForeignKey(e => e.OrganizerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<EventParticipant>(entity =>
            {
                entity.HasKey(ep => new { ep.EventId, ep.UserId });

                entity.HasOne(ep => ep.Event)
                    .WithMany(e => e.Participants)
                    .HasForeignKey(ep => ep.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<ApplicationUser>()
                    .WithMany(u => u.ParticipatingEvents)
                    .HasForeignKey(ep => ep.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FullName)
                    .HasMaxLength(200);
            });
        }
    }
}