using IO.Swagger;

using Microsoft.IdentityModel.Tokens;

using Model;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebAPI
{
	public static class GenerateJwtTokenExtension
	{
		public static string GenerateJwtToken(this IConfiguration configuration, User user)
		{
			SecurityToken token = new JwtSecurityToken(
				issuer: AuthOptions.ISSUER,
				audience: "MyAuthClient",
				claims: new List<Claim> { new Claim("id", user.Id.ToString()) },
				expires: DateTime.UtcNow.AddDays(7),
				signingCredentials: new SigningCredentials(
					AuthOptions.GetSymmetricSecurityKey(),
					SecurityAlgorithms.HmacSha256
				)
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
