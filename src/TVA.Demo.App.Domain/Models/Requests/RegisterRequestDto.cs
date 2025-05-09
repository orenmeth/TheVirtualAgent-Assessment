using System.ComponentModel.DataAnnotations;

namespace TVA.Demo.App.Domain.Models.Requests
{
    public class RegisterRequestDto
    {
        [Required, MinLength(3)] public required string Username { get; set; }
        [Required, EmailAddress] public required string Email { get; set; }
        [Required, MinLength(8)] public required string Password { get; set; }
    }
}
