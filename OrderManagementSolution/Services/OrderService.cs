using AutoMapper;
using OrderManagementSolution.Data;
using OrderManagementSolution.DTOs;
using OrderManagementSolution.Models;
using OrderManagementSolution.Repositories;
using Microsoft.EntityFrameworkCore;

namespace OrderManagementSolution.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly OrderDbContext _context;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, OrderDbContext context)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _context = context;
        }
        public async Task<OrderDto> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            return order != null ? _mapper.Map<OrderDto>(order) : null;
        }

        public async Task<PagedOrderResultDto> GetOrdersAsync(Guid? customerId, OrderStatus? status, DateTime? fromDate, DateTime? toDate, int page, int pageSize)
        {
            var result = await _orderRepository.GetOrdersAsync(customerId, status, fromDate, toDate, page, pageSize);
            return new PagedOrderResultDto
            {
                TotalItems = result.TotalItems,
                Page = page,
                PageSize = pageSize,
                Data = _mapper.Map<List<OrderDto>>(result.Orders)
            };
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            // Validate CustomerId exists
            if (!await _context.Customers.AnyAsync(c => c.CustomerId == createOrderDto.CustomerId))
                throw new ArgumentException("Invalid CustomerId");

            // Validate product IDs exist and deduplicate items
            var productIds = createOrderDto.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _context.Products.Where(p => productIds.Contains(p.ProductId)).ToListAsync();

            if (products.Count != productIds.Count)
                throw new ArgumentException("One or more ProductIds are invalid");

            var order = _mapper.Map<Order>(createOrderDto);
            order.OrderId = Guid.NewGuid();
            order.OrderDate = DateTime.UtcNow;
            order.Status = OrderStatus.Pending;

            // Calculate total and set unit prices
            order.TotalAmount = createOrderDto.Items.Sum(i => i.Quantity * products.First(p => p.ProductId == i.ProductId).Price);

            foreach (var item in order.Items)
            {
                item.OrderItemId = Guid.NewGuid();
                item.UnitPrice = products.First(p => p.ProductId == item.ProductId).Price;
            }

            await _orderRepository.CreateOrderAsync(order);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found.");
            if (!IsValidStatusTransition(order.Status, status))
                throw new Exception($"Invalid status transition from {order.Status} to {status}.");
            await _orderRepository.UpdateOrderStatusAsync(orderId, status);
        }

        private bool IsValidStatusTransition(OrderStatus current, OrderStatus next)
        {
            return (current == OrderStatus.Pending && (next == OrderStatus.Processing || next == OrderStatus.Cancelled)) ||
                   (current == OrderStatus.Processing && (next == OrderStatus.Completed || next == OrderStatus.Cancelled));
        }
    }
}
