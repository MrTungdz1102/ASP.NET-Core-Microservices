using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Gateway.OcelotGateway.Configuration
{
	public static class WebApplicationBuilderExtensions
	{
		public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
		{
			var Secretkey = builder.Configuration["JWT:SecretKey"];
			var Issuer = builder.Configuration["JWT:Issuer"];
			var Audience = builder.Configuration["JWT:Audience"];
			int DurationInMinutes = int.Parse(builder.Configuration["JWT:DurationInMinutes"]);

			var key = Encoding.ASCII.GetBytes(Secretkey);

			builder.Services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(x =>
			{
				x.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = true,
					ValidAudience = Audience,
					ValidIssuer = Issuer,
					ValidateAudience = true
				};
			});
			return builder;
		}
	}
}
