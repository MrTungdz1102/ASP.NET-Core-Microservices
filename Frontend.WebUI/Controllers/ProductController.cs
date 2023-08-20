using Frontend.WebUI.Models.DTOs;
using Frontend.WebUI.Services.Implementation;
using Frontend.WebUI.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Frontend.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> Index()
        {
            List<ProductDTO>? products = new List<ProductDTO>();
            ResponseDTO? response = await _productService.GetAllProduct();
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

        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDTO productDTO)
        {
            ResponseDTO? response = await _productService.CreateProduct(productDTO);
            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Product created successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(productDTO);
        }

        public async Task<IActionResult> UpdateProduct(int id)
        {
            ProductDTO? product = null;
            ResponseDTO? response = await _productService.GetProductById(id);
            if (response is not null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(int id, ProductDTO productDTO)
        {
            ResponseDTO? response = await _productService.UpdateProduct(id, productDTO);
            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Product updated successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(productDTO);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            ResponseDTO? response = await _productService.DeleteProduct(id);
            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Product deleted successfully";
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
