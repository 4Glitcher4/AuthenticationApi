using AuthenticationApi.ModelsDto;

namespace AuthenticationApi.Services
{
    public interface IUserService
    {
        Task<string> CreateToken(UserDto userDto);
        Task<string> RefreshToken(string refreshToken);
    }
}