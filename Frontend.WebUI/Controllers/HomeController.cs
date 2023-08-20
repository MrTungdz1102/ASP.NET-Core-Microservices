using Frontend.WebUI.Models;
using Frontend.WebUI.Models.DTOs;
using Frontend.WebUI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Frontend.WebUI.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
		{
			_logger = logger;
			_productService = productService;
		}

		public async Task<IActionResult> Index()
		{
			List<ProductDTO>? products = new List<ProductDTO>();
			ResponseDTO response = await _productService.GetAllProduct();
            if (response is not null && response.IsSuccess)
            {
                products = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ProductDTO? model = new();

            ResponseDTO? response = await _productService.GetProductById(productId);

            if (response != null && response.IsSuccess)
            {
                model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}