using BrainWave.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BrainWave.Api.Data;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
	}

	public DbSet<TaskItem> Tasks => Set<TaskItem>();
	public DbSet<User> Users => Set<User>();
	public DbSet<Reminder> Reminders => Set<Reminder>();
	public DbSet<Badge> Badges => Set<Badge>();
}

public static class DbSeeder
{
	public static void Seed(ApplicationDbContext db)
	{
		if (!db.Users.Any())
		{
			var user = new User { Id = "demo-user", Email = "demo@example.com", DisplayName = "Demo User", Role = UserRole.User };
			db.Users.Add(user);
		}

		if (!db.Tasks.Any())
		{
			db.Tasks.Add(new TaskItem
			{
				Title = "Welcome to BrainWave",
				Description = "This is your first task. You can edit or complete it.",
				OwnerId = "demo-user",
				DueAtUtc = DateTime.UtcNow.AddDays(3)
			});
		}

		db.SaveChanges();
	}
}
