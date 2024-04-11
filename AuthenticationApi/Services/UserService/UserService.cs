using AuthenticationApi.ModelsDto;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthenticationApi.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserSettings _userSettings;

        public UserService(IHttpContextAccessor httpContextAccessor,
            IUserSettings userSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            _userSettings = userSettings;

        }
        public string CreateToken(TokenSettings tokenSettings)
        {
            try
            {

                var securityKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_userSettings.SecretKey));

                var signingCredentials = new SigningCredentials(
                    securityKey, SecurityAlgorithms.HmacSha256);

                var claimsForToken = new List<Claim>();

                foreach (var claim in tokenSettings.GetType().GetProperties())
                {
                    var value = claim.GetValue(tokenSettings) as string;
                    if (value != null)
                    {
                        Enum.TryParse(claim.Name, out ClaimType result);
                        claimsForToken.Add(new Claim(result.ToString(), value));
                    }
                }

                var jwtSecurityToken = new JwtSecurityToken(
                    claims: claimsForToken,
                    notBefore: DateTime.UtcNow,
                    expires: tokenSettings.TokenLifeTime,
                    signingCredentials: signingCredentials);

                var token = new JwtSecurityTokenHandler()
                    .WriteToken(jwtSecurityToken);

                return token;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public string RefreshToken(string refreshToken)
        {
            try
            {
                var principal = GetPrincipal(refreshToken);
                var token = CreateToken(
                  new TokenSettings
                  {
                      UserId = principal.Claims.FirstOrDefault(c => c.Type == ClaimType.UserId.ToString())?.Value,
                      TokenLifeTime = DateTime.UtcNow.AddDays(1)
                  });
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Claims
        public string GetClaimValue(ClaimType tokenEnum)
        {
            try
            {
                if (_httpContextAccessor.HttpContext is not null)
                    return _httpContextAccessor.HttpContext.User.FindFirstValue(tokenEnum.ToString());

                return string.Empty;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string GetClaimValue(string token, ClaimType claimType)
        {
            try
            {
                var principal = GetPrincipal(token);
                return principal.Claims.FirstOrDefault(c => c.Type == claimType.ToString())?.Value;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private ClaimsPrincipal GetPrincipal(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_userSettings.SecretKey)),
                ValidateLifetime = false
            };

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out var validatedToken);

            return principal;
        }

        public string Encrypt(string plainText)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Convert.FromBase64String(_userSettings.Key);
                    aesAlg.IV = Convert.FromBase64String(_userSettings.Iv);

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plainText);
                            }
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Decrypt(string cipherText)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Convert.FromBase64String(_userSettings.Key);
                    aesAlg.IV = Convert.FromBase64String(_userSettings.Iv);

                    byte[] cipherBytes = Convert.FromBase64String(cipherText);

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(cipherBytes, 0, cipherBytes.Length))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
