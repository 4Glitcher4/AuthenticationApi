using AuthenticationApi.ModelsDto;

namespace AuthenticationApi.Services
{
    public class UserService : IUserService
    {
        public Task<string> CreateToken(UserDto userDto)
        {
            throw new NotImplementedException();
        }

        public Task<string> RefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
