using Microsoft.AspNetCore.Mvc;
using OrderManagementSolution.Data;
using OrderManagementSolution.Models;
using Microsoft.EntityFrameworkCore;
using OrderManagementSolution.DTOs;

namespace OrderManagementSolution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly OrderDbContext _context;

        public ProductsController(OrderDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Product>>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(ApiResponse<List<Product>>.Success(products));
        }
    }
}
