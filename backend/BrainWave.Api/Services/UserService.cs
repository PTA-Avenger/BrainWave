using BrainWave.Api.Data;
using BrainWave.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BrainWave.Api.Services;

public interface IUserService
{
	Task<List<User>> GetAllAsync(CancellationToken ct);
	Task<User?> GetByIdAsync(string id, CancellationToken ct);
	Task<User> CreateAsync(User user, CancellationToken ct);
}

public class UserService : IUserService
{
	private readonly ApplicationDbContext _db;

	public UserService(ApplicationDbContext db)
	{
		_db = db;
	}

	public async Task<List<User>> GetAllAsync(CancellationToken ct) =>
		await _db.Users.AsNoTracking().OrderBy(u => u.DisplayName).ToListAsync(ct);

	public async Task<User?> GetByIdAsync(string id, CancellationToken ct) =>
		await _db.Users.FindAsync(new object?[] { id }, ct);

	public async Task<User> CreateAsync(User user, CancellationToken ct)
	{
		_db.Users.Add(user);
		await _db.SaveChangesAsync(ct);
		return user;
	}
}
