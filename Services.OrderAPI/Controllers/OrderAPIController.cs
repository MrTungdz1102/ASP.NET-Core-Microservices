using AutoMapper;
using Integration.MessageBus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.OrderAPI.Data;
using Services.OrderAPI.Models;
using Services.OrderAPI.Models.DTOs;
using Services.OrderAPI.Service.Interface;
using Stripe.Checkout;

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
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public OrderAPIController(AppDbContext context, IMapper mapper, IProductService productService, IConfiguration configuration, IMessageBus messageBus)
        {
            _context = context;
            _mapper = mapper;
            _productService = productService;
            _response = new ResponseDTO();
            _configuration = configuration;
            _messageBus = messageBus;
        }

        [HttpPost("CreateOrder")]
        public async Task<ResponseDTO> CreateOrder([FromBody] CartDTO cartDTO)
        {
            try
            {
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
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDTO> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDTO)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDTO.ApprovedUrl,
                    CancelUrl = stripeRequestDTO.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment"
                  
                };
                var DiscountsObj = new List<SessionDiscountOptions>
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDTO.OrderHeader.CouponCode
                    }
                };
                foreach (var item in stripeRequestDTO.OrderHeader.OrderDetails)
                {
                    var sessionItems = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "USD",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName,

                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionItems);
                }

                if(stripeRequestDTO.OrderHeader.Discount>0)
                {
                    options.Discounts = DiscountsObj;
                }

                var service = new SessionService();
                Session session = service.Create(options);
                stripeRequestDTO.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = await _context.OrderHeaders.FirstAsync(x => x.OrderHeaderId == stripeRequestDTO.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                await _context.SaveChangesAsync();
                _response.Result = stripeRequestDTO;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDTO> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = await _context.OrderHeaders.FirstAsync(x => x.OrderHeaderId == orderHeaderId);
                var service = new SessionService();
                Session session = await service.GetAsync(orderHeader.StripeSessionId);

                var paymentIntentService = new Stripe.PaymentIntentService();
                Stripe.PaymentIntent paymentIntent = await paymentIntentService.GetAsync(session.PaymentIntentId);

                if(paymentIntent.Status == "succeeded")
                {
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = Utility.Constants.Status_Approved;
                    await _context.SaveChangesAsync();

                    // handle azure topic
                    TopicDTO topicDTO = new()
                    {
                        OrderId = orderHeader.OrderHeaderId,
                        UserId = orderHeader.UserId,

                        // mua 1$ = 1 diem tich luy
                        TopicActivity = Convert.ToInt32(orderHeader.OrderTotal)
                    };
                    // _configuration["TopicAndQueueNames:OrderCreatedTopic"]
                    string topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    await _messageBus.PublishMessage(topicDTO, topicName);

                    _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
                }
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [Authorize]
        [HttpGet("GetAllOrder")]
        public async Task<ResponseDTO> GetAllOrder(string? userId = "")
        {
            // neu khong co id va roles la admin thi co the xem toan bo order
            try
            {
                IEnumerable<OrderHeader> listOderHeader = null;
                if (User.IsInRole("ADMIN"))
                {
                    listOderHeader = await _context.OrderHeaders.Include(x => x.OrderDetails).OrderByDescending(x => x.OrderHeaderId).ToListAsync();
                }
                else if(!string.IsNullOrEmpty(userId))
                {
                    listOderHeader = await _context.OrderHeaders.Include(x => x.OrderDetails).Where(x => x.UserId == userId).OrderByDescending(x => x.OrderHeaderId).ToListAsync();                
                }
                _response.Result = _mapper.Map<List<OrderHeaderDTO>>(listOderHeader);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpGet("GetOrderById/{id:int}")]
        public async Task<ResponseDTO> GetOrderById(int id)
        {
            // neu khong co id va roles la admin thi co the xem toan bo order
            try
            {
                OrderHeader orderHeader = await _context.OrderHeaders.Include(x => x.OrderDetails).FirstAsync(x => x.OrderHeaderId == id);
                _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDTO>  UpdateOrderStatus([FromRoute] int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader = await _context.OrderHeaders.FirstAsync(x => x.OrderHeaderId == orderId);
                if(orderHeader is not null)
                {
                    if(newStatus == Utility.Constants.Status_Cancelled)
                    {
                        // refund money
                        var options = new Stripe.RefundCreateOptions
                        {
                            Reason = Stripe.RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderHeader.PaymentIntentId
                        };
                        var service = new Stripe.RefundService();
                        Stripe.Refund refund = await service.CreateAsync(options);
                        
                    }
                    orderHeader.Status = newStatus;
                    await _context.SaveChangesAsync();
                }
            }
            catch(Exception ex) {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
