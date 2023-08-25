using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.OrderAPI.Data;
using Services.OrderAPI.Models;
using Services.OrderAPI.Models.DTOs;
using Services.OrderAPI.Service.Interface;

namespace Services.OrderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDTO _response;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly AppDbContext _context;

        public OrderAPIController(AppDbContext context, IMapper mapper, IProductService productService)
        {
            _context = context;
            _mapper = mapper;
            _productService = productService;
            _response = new ResponseDTO();
        }

        [HttpPost("CreateOrder")]
        public async Task<ResponseDTO> CreateOrder([FromBody] CartDTO cartDTO)
        {
            try { 
                OrderHeaderDTO orderHeaderDTO = _mapper.Map<OrderHeaderDTO>(cartDTO.CartHeader);
                orderHeaderDTO.OrderTime = DateTime.Now;
                orderHeaderDTO.Status = Utility.Constants.Status_Pending;
                orderHeaderDTO.OrderDetails = _mapper.Map<List<OrderDetailDTO>>(cartDTO.CartDetails);

                // lay ra luon order vua duoc them vao
                OrderHeader orderHeaderCreated = _context.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDTO)).Entity;
                await _context.SaveChangesAsync();
                // id orderheader tu dong duoc sinh ra nen sau khi add và save thi moi lay ra duoc de su dung
                orderHeaderDTO.OrderHeaderId = orderHeaderCreated.OrderHeaderId;
                _response.Result = orderHeaderDTO;
            }
            catch(Exception ex) { 
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
