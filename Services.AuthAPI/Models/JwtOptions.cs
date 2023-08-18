namespace Services.AuthAPI.Models
{
	public class JwtOptions
	{

		public string Issuer { get; set; } = string.Empty;

		public string Audience { get; set; } = string.Empty;

		public string SecretKey { get; set; } = string.Empty;

		public int DurationInMinutes { get; set; }
	}
}
