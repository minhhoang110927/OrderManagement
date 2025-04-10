using Microsoft.EntityFrameworkCore;
using OrderManagementSolution.Data;
using OrderManagementSolution.Models;

namespace OrderManagementSolution.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
        public async Task<PagedOrderResult> GetOrdersAsync(Guid? customerId, OrderStatus? status, DateTime? fromDate, DateTime? toDate, int page, int pageSize)
        {
            var query = _context.Orders.Include(o => o.Items).AsQueryable();

            if (customerId.HasValue)
                query = query.Where(o => o.CustomerId == customerId.Value);
            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);
            if (fromDate.HasValue)
                query = query.Where(o => o.OrderDate >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(o => o.OrderDate <= toDate.Value);

            var totalItems = await query.CountAsync();
            var orders = await query.OrderByDescending(o => o.OrderDate).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedOrderResult
            {
                TotalItems = totalItems,
                Orders = orders
            };
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
        }
    }
}
