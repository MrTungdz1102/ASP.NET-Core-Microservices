using Frontend.WebUI.Models.DTOs;
using System.Threading.Tasks;

namespace Frontend.WebUI.Services.Interface
{
	public interface IBaseService
	{
		Task<ResponseDTO?> SendAsync(RequestDTO requestDTO);
	}
}
