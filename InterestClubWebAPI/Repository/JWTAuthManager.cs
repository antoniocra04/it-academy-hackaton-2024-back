using InterestClubWebAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;



namespace InterestClubWebAPI.Repository
{
    public class JWTAuthManager : IJWTAuthManager
    {
        public Response<string> GenerateJWT(User user)
        {
            var response = new Response<string>();
            var securityKey = AuthOptions.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("login", user.Login),
                new Claim("password", user.Password),
                new Claim("role", user.Role.ToString()),
                new Claim("Date", DateTime.Now.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                AuthOptions.ISSUER,
                AuthOptions.AUDIENCE,
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            response.Data = new JwtSecurityTokenHandler().WriteToken(token); // возвращаем access token
            response.code = 200;
            response.message = "Token generated";
            return response;
        }
        public (string login, string password) getUserCreditansFromJWT(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var decodeToken = handler.ReadJwtToken(token);
            var login = decodeToken.Claims.First(claim => claim.Type == "login").Value;
            var password = decodeToken.Claims.First(claim => claim.Type == "password").Value;
            return (login, password);
        }
    }


}
