using GoodHamburger.Application.Src.Errors;
using GoodHamburger.Domain.Src;
using GoodHamburger.Application.Src.Repositories;

namespace GoodHamburger.Application.Src.UseCases;

public class OrderUseCase(IOrderRepository orderRepository, IProductRepository productRepository)
{
    public async Task<OrderWithTotals> CreateAsync(string clientName, List<string> productIds)
    {
        var products = await ResolveAndValidateProductsAsync(productIds);
        ValidateClientName(clientName);

        var totals = CalculateTotals(products);
        var orderToCreate = new Order(string.Empty, clientName.Trim(), totals.TotalFinal, products);

        var created = await orderRepository.CreateAsync(orderToCreate);

        return MapWithTotals(created);
    }

    public async Task<List<OrderWithTotals>> GetAllAsync()
    {
        var orders = await orderRepository.GetAllAsync();
        return orders.Select(MapWithTotals).ToList();
    }

    public async Task<OrderWithTotals> GetByIdAsync(string id)
    {
        var order = await orderRepository.GetByIdAsync(id)
            ?? throw new ResourceNotFoundException($"Pedido '{id}' nao encontrado.");

        return MapWithTotals(order);
    }

    public async Task<OrderWithTotals> UpdateAsync(string id, string clientName, List<string> productIds)
    {
        ValidateClientName(clientName);

        var existing = await orderRepository.GetByIdAsync(id)
            ?? throw new ResourceNotFoundException($"Pedido '{id}' nao encontrado.");

        var products = await ResolveAndValidateProductsAsync(productIds);
        var totals = CalculateTotals(products);

        var orderToUpdate = new Order(existing.id, clientName.Trim(), totals.TotalFinal, products);

        var updated = await orderRepository.UpdateAsync(orderToUpdate)
            ?? throw new ResourceNotFoundException($"Pedido '{id}' nao encontrado.");

        return MapWithTotals(updated);
    }

    public async Task DeleteAsync(string id)
    {
        var deleted = await orderRepository.DeleteAsync(id);

        if (!deleted)
        {
            throw new ResourceNotFoundException($"Pedido '{id}' nao encontrado.");
        }
    }

    private async Task<List<Product>> ResolveAndValidateProductsAsync(List<string> productIds)
    {
        if (productIds is null || productIds.Count == 0)
        {
            throw new InvalidOrderException("Pedido invalido: informe ao menos um produto.");
        }

        if (productIds.Count != productIds.Distinct(StringComparer.OrdinalIgnoreCase).Count())
        {
            throw new DuplicateOrderItemsException("Itens duplicados no pedido nao sao permitidos.");
        }

        var allProducts = await productRepository.GetAllProductsAsync();
        var productsById = allProducts.ToDictionary(p => p.id, StringComparer.OrdinalIgnoreCase);

        var selectedProducts = new List<Product>();

        foreach (var productId in productIds)
        {
            if (!productsById.TryGetValue(productId, out var product))
            {
                throw new InvalidOrderException($"Pedido invalido: produto '{productId}' nao encontrado no cardapio.");
            }

            selectedProducts.Add(product);
        }

        ValidateNoDuplicateProductTypes(selectedProducts);

        return selectedProducts;
    }

    private static void ValidateNoDuplicateProductTypes(List<Product> products)
    {
        var duplicatedType = products
            .GroupBy(p => p.Type)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicatedType is not null)
        {
            throw new DuplicateOrderItemsException(
                $"Itens duplicados no pedido: apenas 1 item do tipo '{duplicatedType.Key}' e permitido.");
        }
    }

    private static void ValidateClientName(string clientName)
    {
        if (string.IsNullOrWhiteSpace(clientName))
        {
            throw new InvalidOrderException("Pedido invalido: nome do cliente e obrigatorio.");
        }
    }

    private static OrderWithTotals MapWithTotals(Order order)
    {
        var totals = CalculateTotals(order.Products);

        return new OrderWithTotals(
            order.id,
            order.ClientName,
            totals.Subtotal,
            totals.Discount,
            totals.TotalFinal,
            order.Products);
    }

    private static OrderTotals CalculateTotals(List<Product> products)
    {
        var subtotal = products.Sum(p => p.Price);
        var discountCalculator = new CalculateDiscount();
        var totalFinal = discountCalculator.Execute(products);
        var discount = subtotal - totalFinal;

        return new OrderTotals(subtotal, discount, totalFinal);
    }

    private sealed record OrderTotals(decimal Subtotal, decimal Discount, decimal TotalFinal);
}

public record OrderWithTotals(
    string Id,
    string ClientName,
    decimal Subtotal,
    decimal Discount,
    decimal TotalFinal,
    List<Product> Products);
