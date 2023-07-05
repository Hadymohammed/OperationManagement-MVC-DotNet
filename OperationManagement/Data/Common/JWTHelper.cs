using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OperationManagement.Data.Common
{
    public class JWTHelper
    {
        private const string Jwtkey = "35f7a951b0494e5a9852d35a33cd6cc8";
        // Generate token
        public static string GenerateJwtToken(string username, string Id)
        {
            var JwtExpireDays = "2";
            var Jwtissuer = "https://localhost:7055";
            var claims = new List<Claim>
                {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, Id),

                };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Convert.ToString(Jwtkey)));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(Convert.ToString(JwtExpireDays)));

            var token = new JwtSecurityToken(
            Convert.ToString(Jwtissuer),
            null,
            claims,
            expires: expires,
            signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string ValidateToken(string token)
        {

            if (token == null)
                return null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Convert.ToString(Jwtkey));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time
                    //ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var jti = jwtToken.Claims.First(claim => claim.Type == "jti").Value;
                var userName = jwtToken.Claims.First(sub => sub.Type == "sub").Value;
                var Id = jwtToken.Claims.First(sub => sub.Type == ClaimTypes.NameIdentifier).Value;
                // return user id from JWT token if validation successful
                return Id;
            }
            catch
            {
                return null;
            }
        }

    }
}
