using AuthenticationApi.ModelsDto;
using AuthenticationApi.Services;

namespace AuthenticationApi.Services
{
    public interface IUserService
    {
        string CreateToken(TokenSettings userDto);
        string RefreshToken(string refreshToken);

        string Encrypt(string plainText);
        string Decrypt(string cipherText);

        string GetClaimValue(ClaimType claimType);
        string GetClaimValue(string token, ClaimType claimType);
    }

    public enum ClaimType
    {
        UserId,
    }
}