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
    [Authorize]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private ResponseDTO _response;
        public ProductAPIController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ResponseDTO();
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
        public async Task<ActionResult<ResponseDTO>> CreateProduct([FromBody] ProductDTO productDTO)
        {
            var product = _mapper.Map<Product>(productDTO);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            _response.Result = _mapper.Map<ProductDTO>(product);
            return _response;
        }

        [HttpPut("UpdateProduct/{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDTO>> UpdateProduct([FromRoute] int id, [FromBody] ProductDTO productDTO)
        {
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
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return _response;
        }
    }
}
