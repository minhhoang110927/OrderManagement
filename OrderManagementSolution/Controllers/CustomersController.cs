using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementSolution.Data;
using OrderManagementSolution.DTOs;
using OrderManagementSolution.Models;
using OrderManagementSolution.Services;

namespace OrderManagementSolution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly OrderDbContext _context;

        public CustomersController(OrderDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Customer>>>> GetCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            return Ok(ApiResponse<List<Customer>>.Success(customers));
        }
    }
}
