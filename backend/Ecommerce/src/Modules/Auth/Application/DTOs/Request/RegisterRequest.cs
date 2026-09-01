namespace Ecommerce.Auth.Application.DTOs.Request;

/// <summary>
/// Data transfer object carrying user registration details for new account creation.
/// </summary>
/// <param name="Email">The unique email address used as the primary login credential.</param>
/// <param name="Password">The plain-text password to be hashed and stored upon registration.</param>
/// <param name="FirstName">The first name of the registering user.</param>
/// <param name="LastName">The last name of the registering user.</param>
/// <param name="Cellphone">An optional primary contact phone number.</param>
/// <param name="Dni">An optional National Identity Document (DNI) number.</param>
public record RegisterRequest(
    string Email, 
    string Password, 
    string FirstName, 
    string LastName, 
    string? Cellphone, 
    string? Dni
);