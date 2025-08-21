using BrainWave.Api.Models;

namespace BrainWave.Api.DTOs;

public record UserResponse(
	string Id,
	string Email,
	string DisplayName,
	UserRole Role
);

public record CreateUserRequest(
	string Email,
	string DisplayName,
	UserRole Role
);
