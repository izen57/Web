using IO.Swagger;

using Logic;

using Microsoft.IdentityModel.Tokens;

using Serilog;

using System.IdentityModel.Tokens.Jwt;

namespace WebAPI
{
	public class JWTMiddleware
	{
		readonly RequestDelegate _next;
		readonly IConfiguration _configuration;

		public JWTMiddleware(RequestDelegate next, IConfiguration configuration)
		{
			_next = next;
			_configuration = configuration;
		}

		public async Task Invoke(HttpContext context, IUserService userService)
		{
			var token = context.Request.Headers["Authorization"]
				.FirstOrDefault()?
				.Split(" ")
				.Last();

			if (token != null)
				AttachUserToContext(context, userService, token);

			await _next(context);
		}

		public void AttachUserToContext(HttpContext context, IUserService userService, string token)
		{
			try
			{
				JwtSecurityTokenHandler tokenHandler = new();
				tokenHandler.ValidateToken(
					token,
					new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
						ValidateIssuer = false,
						ValidateAudience = false,
						ClockSkew = TimeSpan.Zero
					},
					out SecurityToken validatedToken
				);

				JwtSecurityToken jwtToken = (JwtSecurityToken) validatedToken;
				Guid userId = Guid.Parse(jwtToken.Claims.First(id => id.Type == "id").Value);

				context.Items["User"] = userId;
			}
			catch
			{
				Log.Logger.Error("Ошибка JWT");
			}
		}
	}
}
