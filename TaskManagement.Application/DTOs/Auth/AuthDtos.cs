namespace TaskManagement.Application.DTOs.Auth;

public record RegisterDto(
    string FirstName,
    string LastName,
    string Email,
    string Password);

public record LoginDto(
    string Email,
    string Password);

public record AuthResultDto(
    int UserId,
    string Email,
    string FullName,
    string Token,
    DateTime ExpiresAt);
