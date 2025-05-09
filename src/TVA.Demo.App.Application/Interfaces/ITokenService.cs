using TVA.Demo.App.Domain.Entities;

namespace TVA.Demo.App.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        DateTime GetTokenExpiry();
    }
}