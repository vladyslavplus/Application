using Evently.Domain.Entities;
using Evently.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Evently.Infrastructure.Persistence
{
    public static class ApplicationDbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var logger = scopedServices.GetRequiredService<ILoggerFactory>().CreateLogger("ApplicationDbSeeder");
            var context = scopedServices.GetRequiredService<ApplicationDbContext>();
            var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scopedServices.GetRequiredService<RoleManager<ApplicationRole>>();

            await context.Database.MigrateAsync();

            try
            {
                if (!await roleManager.Roles.AnyAsync())
                {
                    var roles = new[]
                    {
                        new ApplicationRole { Name = "Admin", NormalizedName = "ADMIN" },
                        new ApplicationRole { Name = "User", NormalizedName = "USER" }
                    };

                    foreach (var role in roles)
                        await roleManager.CreateAsync(role);

                    logger.LogInformation("Roles seeded successfully");
                }

                var adminEmail = "admin@example.com";
                var userEmail = "user@example.com";

                var admin = await userManager.FindByEmailAsync(adminEmail);
                if (admin == null)
                {
                    admin = new ApplicationUser
                    {
                        UserName = "Admin",
                        Email = adminEmail,
                        FullName = "System Administrator",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    var result = await userManager.CreateAsync(admin, "Admin@1234");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                        logger.LogInformation("Admin user created");
                    }
                    else
                    {
                        logger.LogError("Failed to create admin: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }

                var user = await userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = "User",
                        Email = userEmail,
                        FullName = "John Doe",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    var result = await userManager.CreateAsync(user, "User@1234");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                        logger.LogInformation("Regular user created");
                    }
                    else
                    {
                        logger.LogError("Failed to create user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }

                if (!await context.Events.AnyAsync())
                {
                    var events = new List<Event>
                    {
                        new()
                        {
                            Title = "Tech Conference 2025",
                            Description = "A conference about modern technologies.",
                            StartDate = DateTimeOffset.UtcNow.AddDays(10),
                            EndDate = DateTimeOffset.UtcNow.AddDays(10).AddHours(5),
                            Location = "Kyiv, Ukraine",
                            Capacity = 100,
                            IsPublic = true,
                            OrganizerId = admin.Id
                        },
                        new()
                        {
                            Title = "Anime Meetup",
                            Description = "Otaku community gathering.",
                            StartDate = DateTimeOffset.UtcNow.AddDays(3),
                            EndDate = DateTimeOffset.UtcNow.AddDays(3).AddHours(2),
                            Location = "Chernivtsi, Ukraine",
                            Capacity = 30,
                            IsPublic = true,
                            OrganizerId = user.Id
                        },
                        new()
                        {
                            Title = "Private Project Brainstorm",
                            Description = "Internal meeting for developers.",
                            StartDate = DateTimeOffset.UtcNow.AddDays(5),
                            EndDate = DateTimeOffset.UtcNow.AddDays(5).AddHours(3),
                            Location = "Online (Zoom)",
                            Capacity = null,
                            IsPublic = false,
                            OrganizerId = admin.Id
                        }
                    };

                    await context.Events.AddRangeAsync(events);
                    await context.SaveChangesAsync();

                    logger.LogInformation("Sample events created successfully");

                    var techConference = events.First(e => e.Title == "Tech Conference 2025");
                    var animeMeetup = events.First(e => e.Title == "Anime Meetup");

                    var participants = new List<EventParticipant>
                    {
                        new() { EventId = techConference.Id, UserId = user.Id, JoinedAt = DateTimeOffset.UtcNow },
                        new() { EventId = animeMeetup.Id, UserId = admin.Id, JoinedAt = DateTimeOffset.UtcNow },
                        new() { EventId = animeMeetup.Id, UserId = user.Id, JoinedAt = DateTimeOffset.UtcNow }
                    };

                    await context.EventParticipants.AddRangeAsync(participants);
                    await context.SaveChangesAsync();

                    logger.LogInformation("Event participants seeded successfully");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during database seeding");
            }
        }
    }
}