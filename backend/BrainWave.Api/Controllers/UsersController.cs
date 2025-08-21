using BrainWave.Api.DTOs;
using BrainWave.Api.Models;
using BrainWave.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrainWave.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
	private readonly IUserService _users;

	public UsersController(IUserService users)
	{
		_users = users;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll(CancellationToken ct)
	{
		var list = await _users.GetAllAsync(ct);
		return Ok(list.Select(u => new UserResponse(u.Id, u.Email, u.DisplayName, u.Role)));
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<UserResponse>> GetById(string id, CancellationToken ct)
	{
		var user = await _users.GetByIdAsync(id, ct);
		if (user is null) return NotFound();
		return Ok(new UserResponse(user.Id, user.Email, user.DisplayName, user.Role));
	}

	[HttpPost]
	public async Task<ActionResult<UserResponse>> Create(CreateUserRequest request, CancellationToken ct)
	{
		var created = await _users.CreateAsync(new User
		{
			Email = request.Email,
			DisplayName = request.DisplayName,
			Role = request.Role
		}, ct);
		return CreatedAtAction(nameof(GetById), new { id = created.Id }, new UserResponse(created.Id, created.Email, created.DisplayName, created.Role));
	}
}
