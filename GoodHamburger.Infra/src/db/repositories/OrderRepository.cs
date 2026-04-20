using GoodHamburger.Application.Src.Repositories;
using GoodHamburger.Domain.Src;
using GoodHamburger.Infra.Src.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infra.Src.Db.Repositories;

public class OrderRepository(AppDbContext dbContext) : IOrderRepository
{
    public async Task<Order> CreateAsync(Order order)
    {
        var orderId = string.IsNullOrWhiteSpace(order.id) ? Guid.NewGuid() : Guid.Parse(order.id);

        var orderEntity = new OrderEntity
        {
            Id = orderId,
            ClientName = order.ClientName,
            TotalPrice = order.TotalPrice
        };

        var productLinks = order.Products
            .Select(p => new OrderProductEntity
            {
                OrderId = orderId,
                ProductId = Guid.Parse(p.id)
            })
            .ToList();

        dbContext.Orders.Add(orderEntity);
        dbContext.OrderProducts.AddRange(productLinks);

        await dbContext.SaveChangesAsync();

        return await GetRequiredByIdAsync(orderId);
    }

    public async Task<Order?> GetByIdAsync(string id)
    {
        if (!Guid.TryParse(id, out var orderId))
        {
            return null;
        }

        return await GetOrderByIdAsync(orderId);
    }

    public async Task<List<Order>> GetAllAsync()
    {
        var orderEntities = await dbContext.Orders
            .AsNoTracking()
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .ToListAsync();

        return orderEntities.Select(MapToDomain).ToList();
    }

    private async Task<Order> GetRequiredByIdAsync(Guid id)
    {
        var order = await GetOrderByIdAsync(id)
            ?? throw new InvalidOperationException($"Order '{id}' was not found after save.");

        return order;
    }

    private async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        var orderEntity = await dbContext.Orders
            .AsNoTracking()
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        return orderEntity is null ? null : MapToDomain(orderEntity);
    }

    private static Order MapToDomain(OrderEntity orderEntity)
    {
        var products = orderEntity.OrderProducts
            .Select(link => new Product(
                link.Product.Id.ToString(),
                link.Product.Name,
                link.Product.Price,
                ParseProductType(link.Product.Category)))
            .ToList();

        return new Order(
            orderEntity.Id.ToString(),
            orderEntity.ClientName,
            orderEntity.TotalPrice,
            products);
    }

    private static ProductType ParseProductType(string category)
    {
        return category switch
        {
            "Hamburger" => ProductType.Hamburger,
            "Fries" => ProductType.Fries,
            "Drink" => ProductType.Drink,
            _ => throw new InvalidOperationException($"Unknown product category '{category}'.")
        };
    }
}
