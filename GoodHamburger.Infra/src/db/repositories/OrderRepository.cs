using GoodHamburger.Domain.Src;
using GoodHamburger.Application.Src.Repositories;
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
            Subtotal = order.Subtotal,
            Discount = order.Discount,
            TotalPrice = order.TotalPrice
        };

        var productLinks = order.Products
            .Select(p => new OrderProductEntity
            {
                OrderId = orderId,
                ProductId = Guid.Parse(p.id),
                UnitPrice = p.Price
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

    public async Task<Order?> UpdateAsync(Order order)
    {
        if (!Guid.TryParse(order.id, out var orderId))
        {
            return null;
        }

        var orderEntity = await dbContext.Orders
            .Include(o => o.OrderProducts)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (orderEntity is null)
        {
            return null;
        }

        orderEntity.ClientName = order.ClientName;
        orderEntity.Subtotal = order.Subtotal;
        orderEntity.Discount = order.Discount;
        orderEntity.TotalPrice = order.TotalPrice;

        dbContext.OrderProducts.RemoveRange(orderEntity.OrderProducts);

        var productLinks = order.Products
            .Select(p => new OrderProductEntity
            {
                OrderId = orderId,
                ProductId = Guid.Parse(p.id),
                UnitPrice = p.Price
            })
            .ToList();

        dbContext.OrderProducts.AddRange(productLinks);

        await dbContext.SaveChangesAsync();

        return await GetOrderByIdAsync(orderId);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!Guid.TryParse(id, out var orderId))
        {
            return false;
        }

        var orderEntity = await dbContext.Orders
            .Include(o => o.OrderProducts)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (orderEntity is null)
        {
            return false;
        }

        dbContext.OrderProducts.RemoveRange(orderEntity.OrderProducts);
        dbContext.Orders.Remove(orderEntity);

        await dbContext.SaveChangesAsync();

        return true;
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
                link.UnitPrice,
                ParseProductType(link.Product.Category)))
            .ToList();

        return new Order(
            orderEntity.Id.ToString(),
            orderEntity.ClientName,
            orderEntity.Subtotal,
            orderEntity.Discount,
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
