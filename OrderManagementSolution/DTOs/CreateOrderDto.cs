using System.ComponentModel.DataAnnotations;

namespace OrderManagementSolution.DTOs
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "CustomerId is required")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "Items are required")]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<OrderItemDto> Items { get; set; }
    }
}
