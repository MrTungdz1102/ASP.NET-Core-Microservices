using Frontend.WebUI.Models.DTOs;
using Frontend.WebUI.Services.Interface;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static Frontend.WebUI.Utility.Constants;

namespace Frontend.WebUI.Services.Implementation
{
	public class BaseService : IBaseService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		public BaseService(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}
		public async Task<ResponseDTO?> SendAsync(RequestDTO requestDTO)
		{
			try
			{
				HttpClient client = _httpClientFactory.CreateClient("API");
				HttpRequestMessage message = new HttpRequestMessage();

				// ưu tiên nhận dữ liệu dưới định dạng JSON nếu có sẵn
				// client vẫn sẽ nhận được dữ liệu nếu dữ liệu trả về khác với json
				message.Headers.Add("Accept", "application/json");

				message.RequestUri = new Uri(requestDTO.Url);
				if (requestDTO.Data != null)
				{
					message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");
				}
				HttpResponseMessage? responseMessage = null;
				switch (requestDTO.ApiType)
				{
					case ApiType.POST:
						message.Method = HttpMethod.Post;
						break;
					case ApiType.DELETE:
						message.Method = HttpMethod.Delete;
						break;
					case ApiType.PUT:
						message.Method = HttpMethod.Put;
						break;
					default:
						message.Method = HttpMethod.Get;
						break;
				}
				responseMessage = await client.SendAsync(message);
				switch (responseMessage.StatusCode)
				{
					case HttpStatusCode.NotFound:
						return new() { IsSuccess = false, Message = "Not Found" };
					case HttpStatusCode.Forbidden:
						return new() { IsSuccess = false, Message = "Access Denied" };
					case HttpStatusCode.Unauthorized:
						return new() { IsSuccess = false, Message = "Unauthorized" };
					case HttpStatusCode.InternalServerError:
						return new() { IsSuccess = false, Message = "Internal Server Error" };
					default:
						var apiContent = await responseMessage.Content.ReadAsStringAsync();
						var apiResponseDto = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
						return apiResponseDto;
				}
			}
			catch (Exception ex)
			{
				var dto = new ResponseDTO
				{
					Message = ex.Message.ToString(),
					IsSuccess = false
				};
				return dto;
			}
		}
	}
}
