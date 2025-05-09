using System.ComponentModel.DataAnnotations;

namespace TVA.Demo.App.Domain.Models.Requests
{
    public class LoginRequestDto
    {
        [Required] public required string UsernameOrEmail { get; set; }
        [Required] public required string Password { get; set; }
    }
}
