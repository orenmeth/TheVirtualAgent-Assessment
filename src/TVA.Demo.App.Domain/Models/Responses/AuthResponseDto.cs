namespace TVA.Demo.App.Domain.Models.Responses
{
    public class AuthResponseDto
    {
        public required string Token { get; set; }
        public DateTime Expiration { get; set; }
        public required string Username { get; set; }
    }
}
