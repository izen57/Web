using IO.Swagger;

using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

using Model;

using Serilog;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebAPI
{
	public class JWTMiddleware
	{
		readonly RequestDelegate _next;
		readonly IServiceProvider _services;

		public JWTMiddleware(RequestDelegate next, IServiceProvider services)
		{
			_next = next;
			_services = services;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var token = context.Request.Headers["Authorization"]
				.FirstOrDefault()?
				.Split(" ")
				.Last();

			if (token != null)
				AttachUserToContext(context, token);

			await _next(context);
		}

		public void AttachUserToContext(HttpContext context, string token)
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
				Log.Logger.Information($"from JWT: {userId}.");

				context.Items["User ID"] = userId;
				_services.GetService<Dictionary<Guid, Stopwatch>>().TryAdd(userId, new Stopwatch(
					"Секундомер",
					System.Drawing.Color.White,
					new System.Diagnostics.Stopwatch(),
					new SortedSet<DateTime>(),
					false
				));
			}
			catch
			{
				Log.Logger.Error("Ошибка JWT");
			}
		}

		public static string GenerateJwtToken(User user)
		{
			SecurityToken token = new JwtSecurityToken(
				issuer: AuthOptions.ISSUER,
				audience: AuthOptions.AUDIENCE,
				claims: new List<Claim> { new Claim("id", user.Id.ToString()) },
				expires: DateTime.UtcNow.AddDays(7),
				signingCredentials: new SigningCredentials(
					AuthOptions.GetSymmetricSecurityKey(),
					SecurityAlgorithms.HmacSha256
				)
			);

			return "Bearer " + new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
