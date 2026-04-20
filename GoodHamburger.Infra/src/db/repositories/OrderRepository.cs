using GoodHamburger.Domain.Src;
using GoodHamburger.Application.Src.Repositories;
using GoodHamburger.Infra.Src.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace GoodHamburger.Infra.Src.Db.Repositories;

public class OrderRepository(AppDbContext dbContext, HybridCache hybridCache) : IOrderRepository
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private const string OrdersAllCacheKey = "orders:all:v1";

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
        await hybridCache.RemoveAsync(OrdersAllCacheKey);

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
        return await hybridCache.GetOrCreateAsync(
            OrdersAllCacheKey,
            async _ =>
            {
                var orderEntities = await dbContext.Orders
                    .AsNoTracking()
                    .Where(o => o.DeletedAt == null)
                    .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                    .ThenInclude(p => p.Category)
                    .ToListAsync();

                return orderEntities.Select(MapToDomain).ToList();
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = CacheDuration,
                LocalCacheExpiration = CacheDuration
            });
    }

    public async Task<Order?> UpdateAsync(Order order)
    {
        if (!Guid.TryParse(order.id, out var orderId))
        {
            return null;
        }

        var orderEntity = await dbContext.Orders
            .Include(o => o.OrderProducts)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.DeletedAt == null);

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
        await hybridCache.RemoveAsync(OrdersAllCacheKey);

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
            .FirstOrDefaultAsync(o => o.Id == orderId && o.DeletedAt == null);

        if (orderEntity is null)
        {
            return false;
        }

        orderEntity.DeletedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync();
        await hybridCache.RemoveAsync(OrdersAllCacheKey);

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
            .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(o => o.Id == id && o.DeletedAt == null);

        return orderEntity is null ? null : MapToDomain(orderEntity);
    }

    private static Order MapToDomain(OrderEntity orderEntity)
    {
        var products = orderEntity.OrderProducts
            .Select(link => new Product(
                link.Product.Id.ToString(),
                link.Product.Name,
                link.UnitPrice,
                link.Product.Category.Name))
            .ToList();

        return new Order(
            orderEntity.Id.ToString(),
            orderEntity.ClientName,
            orderEntity.Subtotal,
            orderEntity.Discount,
            orderEntity.TotalPrice,
            products);
    }

}
