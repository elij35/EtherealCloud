using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static StorageController.Controllers.FileGetController;

namespace StorageController.Data
{
    public class AuthManager
    {

        public async static Task<string> AuthorizeUser(int userID)
        {

            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(Program.SECURITY_KEY));
            SigningCredentials signingCredential = new(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] tokenClaims = 
            {
                new Claim("userID", userID.ToString())
            };

            JwtSecurityToken authToken = new(
                issuer: "storage-controller",
                audience: "user",
                claims: tokenClaims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: signingCredential);

            return new JwtSecurityTokenHandler().WriteToken(authToken);

        }

        public static async Task<JwtSecurityToken?> ValidateToken(string token)
        {

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? tokenJWT;

            try
            {
                handler.ValidateToken(token, Program.Validation_Parameters, out SecurityToken tokenValidated);
                tokenJWT = tokenValidated as JwtSecurityToken;
            }
            catch
            {
                return null;
            }

            return tokenJWT;

        }

        public static async Task<int?> GetUserIDFromToken(JwtSecurityToken token)
        {

            int? userID = null;

            try
            {
                userID = int.Parse(token.Claims.FirstOrDefault(claim => claim.Type == "userID").Value);
            }
            catch { }

            return userID;

        }

        public static async Task<Response<string>> AuthorizeUser(string tokenString)
        {

            JwtSecurityToken? token = await AuthManager.ValidateToken(tokenString);

            if (token == null)
            {
                return new Response<string>(false, "Invalid auth token.");
            }

            int? userID = await AuthManager.GetUserIDFromToken(token);

            if (userID == null)
            {
                return new Response<string>(false, "Invalid auth token.");
            }

            return new Response<string>(true, userID.ToString());

        }

    }
}
