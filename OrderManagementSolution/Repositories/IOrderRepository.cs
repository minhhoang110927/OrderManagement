using OrderManagementSolution.Models;

namespace OrderManagementSolution.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task<PagedOrderResult> GetOrdersAsync(Guid? customerId, OrderStatus? status, DateTime? fromDate, DateTime? toDate, int page, int pageSize);
        Task<Order> CreateOrderAsync(Order order);
        Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
    }
    public class PagedOrderResult
    {
        public int TotalItems { get; set; }
        public List<Order> Orders { get; set; }
    }
}
