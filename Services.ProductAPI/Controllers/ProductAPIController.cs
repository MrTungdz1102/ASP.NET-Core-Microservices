using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.ProductAPI.Data;
using Services.ProductAPI.Models;
using Services.ProductAPI.Models.DTOs;

namespace Services.ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private ResponseDTO _response;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IHttpContextAccessor _httpContextAccessor;
		public ProductAPIController(AppDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor
			)
        {
            _context = context;
            _mapper = mapper;
            _response = new ResponseDTO();
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet("GetAllProduct")]
        public async Task<ActionResult<ResponseDTO>> GetAllProduct()
        {
            var products = await _context.Products.ToListAsync();
            if (products is null)
            {
                _response.IsSuccess = false;
                _response.Message = "Cannot find out product";
            }
           _response.Result = _mapper.Map<List<ProductDTO>>(products);
            return _response;
        }

        [HttpGet("GetProductById/{id:int}")]
        public async Task<ActionResult<ResponseDTO>> GetProductById([FromRoute] int id)
        {
           var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                _response.IsSuccess = false;
                _response.Message = "Cannot find out product";
            }
            _response.Result = _mapper.Map<ProductDTO>(product);
            return _response;
        }

        [HttpPost("CreateProduct")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDTO>> CreateProduct([FromForm] ProductDTO productDTO)
        {
			// khi upload file phai su dung fromform, khong duoc formbody tu net 7 tro xuong
			var product = _mapper.Map<Product>(productDTO);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            if(productDTO.Image is not null)
            {
				// Path.GetExtension(productDTO.Image.FileName) trả về phần mở rộng của tên tập tin trong
				// productDTO.Image.FileName. Ví dụ, nếu productDTO.Image.FileName là "image.jpg"
				// thì Path.GetExtension sẽ trả về ".jpg".
				// fileName sẽ được tạo bằng cách ghép ID của sản phẩm và phần mở rộng của tên tập tin, ví dụ: "123.jpg"
				string fileName = product.ProductId + Path.GetExtension(productDTO.Image.FileName);
                string filePath = _webHostEnvironment.WebRootPath + @"/ProductImages/" + fileName;
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await productDTO.Image.CopyToAsync(fileStream);
                }
				//		var urlFilePath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Images/{fileName}";
				// su dung _httpContextAccessor khi khong o trong controller hoac middleware
				var urlFilePath = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}/ProductImages/{fileName}";
                product.ImageUrl = urlFilePath;
                product.ImageLocalPath= filePath;
            }
			else
			{
				product.ImageUrl = "https://placehold.co/600x400";
			}
			// add truoc update sau vi khi add thi productId moi duoc khoi tao, sau do ta moi co the lay ra productId
			// o dong string fileName = product.ProductId + Path.GetExtension(productDTO.Image.FileName);
			_context.Products.Update(product);
			await _context.SaveChangesAsync();
			_response.Result = _mapper.Map<ProductDTO>(product);
            return _response;
        }

        [HttpPut("UpdateProduct/{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDTO>> UpdateProduct([FromRoute] int id, [FromForm] ProductDTO productDTO)
        {
            // khi upload file phai su dung fromform, khong duoc formbody tu net 7 tro xuong
            if (id != productDTO.ProductId)
            {
                return BadRequest();
            }
            var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                _response.IsSuccess = false;
                _response.Message = "Cannot find out the product";
            }
            else
            {
                if (productDTO.Image is not null)
                {
					if (!string.IsNullOrEmpty(product.ImageLocalPath))
					{
						FileInfo fileInfo = new FileInfo(product.ImageLocalPath);
						if (fileInfo.Exists)
						{
							fileInfo.Delete();
						}
					}
					string fileName = product.ProductId + Path.GetExtension(productDTO.Image.FileName);
                    string filePath = _webHostEnvironment.WebRootPath + @"/ProductImages/" + fileName;
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await productDTO.Image.CopyToAsync(fileStream);
                    }
					var urlFilePath = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}/ProductImages/{fileName}";
					productDTO.ImageUrl = urlFilePath;
					productDTO.ImageLocalPath = filePath;
				}
				_mapper.Map(productDTO, product);
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                _response.Result = _mapper.Map<ProductDTO>(product); 
            }
            return _response;
        }

        [HttpDelete("DeleteProduct/{id:}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDTO>> DeleteProduct([FromRoute] int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                _response.IsSuccess = false;
                _response.Message = "Cannot find out the product";
            }
            else
            {
                if (!string.IsNullOrEmpty(product.ImageLocalPath))
                {
                    FileInfo fileInfo = new FileInfo(product.ImageLocalPath);
                    if(fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }
                }
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return _response;
        }
    }
}
