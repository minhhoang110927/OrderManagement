using Microsoft.AspNetCore.Mvc;
using OrderManagementSolution.DTOs;
using OrderManagementSolution.Models;
using OrderManagementSolution.Services;

namespace OrderManagementSolution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrder(Guid orderId)
        {
            if (orderId == Guid.Empty)
                return BadRequest(ApiResponse<OrderDto>.Error("Invalid OrderId"));

            var order = await _orderService.GetOrderByIdAsync(orderId);
            return order != null
                ? Ok(ApiResponse<OrderDto>.Success(order))
                : NotFound(ApiResponse<OrderDto>.Error("Order not found"));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedOrderResultDto>>> GetOrders(
            [FromQuery] Guid? customerId, [FromQuery] OrderStatus? status,
            [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1)
                return BadRequest(ApiResponse<PagedOrderResultDto>.Error("Page must be at least 1"));
            if (pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse<PagedOrderResultDto>.Error("PageSize must be between 1 and 100"));

            var result = await _orderService.GetOrdersAsync(customerId, status, fromDate, toDate, page, pageSize);
            return Ok(ApiResponse<PagedOrderResultDto>.Success(result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<OrderDto>>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<OrderDto>.Error(string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            try
            {
                var order = await _orderService.CreateOrderAsync(createOrderDto);
                return CreatedAtAction(nameof(GetOrder), new { orderId = order.OrderId }, ApiResponse<OrderDto>.Success(order));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<OrderDto>.Error(ex.Message));
            }
        }

        [HttpPut("{orderId}/status")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateOrderStatus(Guid orderId, [FromBody] UpdateOrderStatusDto dto)
        {
            if (orderId == Guid.Empty)
                return BadRequest(ApiResponse<object>.Error("Invalid OrderId"));

            try
            {
                await _orderService.UpdateOrderStatusAsync(orderId, dto.Status);
                return Ok(ApiResponse<object>.Success(null, "Status updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }
    }
}
