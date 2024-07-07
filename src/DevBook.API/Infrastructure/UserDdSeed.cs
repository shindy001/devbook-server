namespace DevBook.API.Infrastructure;

internal sealed record UserDbSeed(string UserRole, string Email, string Password, bool EmailConfirmed = false);
