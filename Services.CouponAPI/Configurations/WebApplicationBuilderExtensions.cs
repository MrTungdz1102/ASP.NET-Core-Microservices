using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Services.CouponAPI.Configurations
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
        {
            var Secretkey = builder.Configuration["ApiSettings:SecretKey"];
            var Issuer = builder.Configuration["ApiSettings:Issuer"];
            var Audience = builder.Configuration["ApiSettings:Audience"];
            int DurationInMinutes = int.Parse(builder.Configuration["ApiSettings:DurationInMinutes"]);

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
