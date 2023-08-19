using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.AuthAPI.Models;
using Services.AuthAPI.Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.AuthAPI.Services.Implementation
{
	public class JwtTokenGenerator : IJwtTokenGenerator
	{
		private readonly JwtOptions _jwtOptions;

		public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
		{
			_jwtOptions = jwtOptions.Value;
		}
		public string GenerateToken(AppUser appUser, IEnumerable<string> roles)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var securityKey = Encoding.ASCII.GetBytes(_jwtOptions.SecretKey);
			var claimList = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
				new Claim(JwtRegisteredClaimNames.Sub, appUser.Id),
				new Claim(JwtRegisteredClaimNames.Name, appUser.UserName)
				//"iss" (Issuer): Tên đơn vị cấp token.
				//"sub"(Subject): Chủ sở hữu của token.
				//"aud"(Audience): Người nhận của token.
				//"exp"(Expiration Time): Thời điểm token hết hạn.
				//"nbf"(Not Before): Thời điểm token có thể sử dụng.
				//"iat"(Issued At): Thời điểm token được cấp.
				//"jti"(JWT ID): Một ID duy nhất cho token.
				//"typ"(Token Type): Loại token.
				// new Claim("uid", _user.Id),
			};

			claimList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Audience = _jwtOptions.Audience,
				Issuer = _jwtOptions.Issuer,
				// Claims = claims khi sử dụng JwtSecurityToken thay vì SecurityTokenDescriptor
				Subject = new ClaimsIdentity(claimList),
				Expires = DateTime.Now.AddMinutes(_jwtOptions.DurationInMinutes),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256Signature)
			};
			//createJwtSecurityToken = CreateToken
			var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
