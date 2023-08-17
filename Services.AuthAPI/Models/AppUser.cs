using Microsoft.AspNetCore.Identity;

namespace Services.AuthAPI.Models
{
	public class AppUser : IdentityUser
	{
		public string Name { get; set; }
	}
}
