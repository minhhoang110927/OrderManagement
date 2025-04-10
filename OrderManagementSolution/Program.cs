using Microsoft.EntityFrameworkCore;
using OrderManagementSolution.Data;
using OrderManagementSolution.Mapper;
using OrderManagementSolution.Models;
using OrderManagementSolution.Repositories;
using OrderManagementSolution.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowMVCApp", policy =>
//    {
//        policy.WithOrigins("https://localhost:5000") // MVC app URL
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();


builder.Services.AddAutoMapper(typeof(MappingProfile));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseCors("AllowMVCApp");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Seed some initial data (optional)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    context.Database.EnsureCreated();
    SeedData(context);
}

app.Run();

// Optional: Seed initial data for testing
void SeedData(OrderDbContext context)
{
    if (!context.Customers.Any())
    {
        context.Customers.AddRange(
            new Customer { CustomerId = Guid.NewGuid(), Name = "Hoang Minh Tran", Email = "hoang@example.com", PhoneNumber = "0982994297" },
            new Customer { CustomerId = Guid.NewGuid(), Name = "Vuong Huynh", Email = "vuong@example.com", PhoneNumber = "0123456789" }
        );

        context.Products.AddRange(
            new Product { ProductId = Guid.NewGuid(), Name = "Laptop", Description = "Laptop Msi", Price = 999.99m },
            new Product { ProductId = Guid.NewGuid(), Name = "Mouse", Description = "Steelseries mouse", Price = 99.99m },
             new Product { ProductId = Guid.NewGuid(), Name = "Monitor", Description = "Edra monitor", Price = 149.99m }
        );

        context.SaveChanges();
    }
}