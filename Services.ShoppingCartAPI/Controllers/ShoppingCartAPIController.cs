using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.ShoppingCartAPI.Data;
using Services.ShoppingCartAPI.Models;
using Services.ShoppingCartAPI.Models.DTOs;
using Services.ShoppingCartAPI.RabbitMqSender;
using Services.ShoppingCartAPI.Service.Interface;
using System.Reflection.PortableExecutable;

namespace Services.ShoppingCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private ResponseDTO _response;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IRabbitMQCartMessage _rabbitMQAuth;
        private readonly IConfiguration _configuration;
        public ShoppingCartAPIController(AppDbContext context, IMapper mapper, IProductService productService,
            ICouponService couponService, IRabbitMQCartMessage rabbitMQAuth, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _response = new ResponseDTO();
            _productService = productService;
            _couponService = couponService;
            _rabbitMQAuth = rabbitMQAuth;
            _configuration = configuration;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDTO> GetCart(string userId)
        {
            try
            {
                CartDTO cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDTO>(_context.CartHeaders.First(u => u.UserId == userId))
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailDTO>>(_context.CartDetails
                    .Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));
                IEnumerable<ProductDTO> productDTOs = await _productService.GetAllProduct();
                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDTOs.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                if(!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDTO couponDTO = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                    if(couponDTO is not null && cart.CartHeader.CartTotal > couponDTO.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= couponDTO.DiscountAmount;
                        cart.CartHeader.Discount = couponDTO.DiscountAmount;
                    }
                }

                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDTO cartDto)
        {
            try
            {
                var cartFromDb = await _context.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                var couponFromDb = await _couponService.GetCoupon(cartDto.CartHeader.CouponCode);
                if (string.IsNullOrEmpty(couponFromDb.CouponCode))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Coupon is not valid";
                }
                else
                {
                    cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                    _context.CartHeaders.Update(cartFromDb);
                    await _context.SaveChangesAsync();
                    _response.Result = true;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();
            }
            return _response;
        }

        [HttpPost("CartUpSert")]
        public async Task<ActionResult<ResponseDTO>> CartUpSert(CartDTO cartDTO)
        {
            var cartFromDb = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDTO.CartHeader.UserId);
            if (cartFromDb == null)
            {
                CartHeader cartHeader = _mapper.Map<CartHeader>(cartDTO.CartHeader);
                _context.CartHeaders.Add(cartHeader);
                await _context.SaveChangesAsync();
                cartDTO.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                _context.CartDetails.Add(_mapper.Map<CartDetail>(cartDTO.CartDetails.First()));
                await _context.SaveChangesAsync();
            }
            else
            {
                // kiem tra co product trong gio hang roi hay chua
                var cartDetailFromDb = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == cartDTO.CartDetails.First().ProductId && x.CartHeaderId == cartFromDb.CartHeaderId);
                if (cartDetailFromDb is null) {
                    // create cartdetail
                    cartDTO.CartDetails.First().CartHeaderId = cartFromDb.CartHeaderId;
                    _context.CartDetails.Add(_mapper.Map<CartDetail>(cartDTO.CartDetails.First()));
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // update count
                    cartDTO.CartDetails.First().Count += cartDetailFromDb.Count;
                    cartDTO.CartDetails.First().CartHeaderId = cartDetailFromDb.CartHeaderId;
                    cartDTO.CartDetails.First().CartDetailId = cartDetailFromDb.CartDetailId;
                    _context.CartDetails.Update(_mapper.Map<CartDetail>(cartDTO.CartDetails.First()));
                    await _context.SaveChangesAsync();
                }
            }
            _response.Result = cartDTO;
            return _response;
        }
        
		[HttpPost("EmailCartRequest")]
		public async Task<object> EmailCartRequest([FromBody] CartDTO cartDto)
		{
			try
			{
                // rabbitmq
				_rabbitMQAuth.SendMessage(cartDto, _configuration["TopicAndQueueNames:EmailShoppingCartQueue"]);
				_response.Result = true;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.ToString();
			}
			return _response;
		}

		[HttpPost("RemoveCart")]
        public async Task<ResponseDTO> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetail cartDetails = _context.CartDetails
                   .First(u => u.CartDetailId == cartDetailsId);

                int totalCountofCartItem = _context.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _context.CartDetails.Remove(cartDetails);
                if (totalCountofCartItem == 1)
                {
                    var cartHeaderToRemove = await _context.CartHeaders
                       .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                    _context.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _context.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }
    }
}
