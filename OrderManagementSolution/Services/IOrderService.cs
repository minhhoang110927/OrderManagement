using OrderManagementSolution.DTOs;
using OrderManagementSolution.Models;

namespace OrderManagementSolution.Services
{
    public interface IOrderService
    {
        Task<OrderDto> GetOrderByIdAsync(Guid orderId);
        Task<PagedOrderResultDto> GetOrdersAsync(Guid? customerId, OrderStatus? status, DateTime? fromDate, DateTime? toDate, int page, int pageSize);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
    }
}
