using GoodHamburger.Application.Src.Errors;
using GoodHamburger.Domain.Src;
using GoodHamburger.Application.Src.Repositories;

namespace GoodHamburger.Application.Src.UseCases;

public class OrderUseCase(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IComboRepository comboRepository,
    DiscountCalculator discountCalculator)
{
    public async Task<OrderWithTotals> CreateAsync(string clientName, List<string> productIds)
    {
        var products = await ResolveAndValidateProductsAsync(productIds);
        ValidateClientName(clientName);

        var totals = await CalculateTotalsAsync(products);
        var orderToCreate = new Order(
            string.Empty,
            clientName.Trim(),
            totals.Subtotal,
            totals.Discount,
            totals.TotalFinal,
            products);

        var created = await orderRepository.CreateAsync(orderToCreate);

        return OrderWithPrices(created);
    }

    public async Task<List<OrderWithTotals>> GetAllAsync()
    {
        var orders = await orderRepository.GetAllAsync();
        return orders.Select(OrderWithPrices).ToList();
    }

    public async Task<OrderWithTotals> GetByIdAsync(string id)
    {
        var order = await orderRepository.GetByIdAsync(id)
            ?? throw new ResourceNotFoundException($"Pedido '{id}' nao encontrado.");

        return OrderWithPrices(order);
    }

    public async Task<OrderWithTotals> UpdateAsync(string id, string clientName, List<string> productIds)
    {
        ValidateClientName(clientName);

        var existing = await orderRepository.GetByIdAsync(id)
            ?? throw new ResourceNotFoundException($"Pedido '{id}' nao encontrado.");

        var products = await ResolveAndValidateProductsAsync(productIds);
        var totals = await CalculateTotalsAsync(products);

        var orderToUpdate = new Order(
            existing.id,
            clientName.Trim(),
            totals.Subtotal,
            totals.Discount,
            totals.TotalFinal,
            products);

        var updated = await orderRepository.UpdateAsync(orderToUpdate)
            ?? throw new ResourceNotFoundException($"Pedido '{id}' nao encontrado.");

        return OrderWithPrices(updated);
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
            .GroupBy(p => p.Category, StringComparer.OrdinalIgnoreCase)
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

    private static OrderWithTotals OrderWithPrices(Order order)
    {
        return new OrderWithTotals(
            order.id,
            order.ClientName,
            order.Subtotal,
            order.Discount,
            order.TotalPrice,
            order.Products);
    }

    private async Task<OrderTotals> CalculateTotalsAsync(List<Product> products)
    {
        var subtotal = products.Sum(p => p.Price);
        var combos = await comboRepository.GetActiveCombosAsync();
        var totalFinal = discountCalculator.Execute(products, combos);
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
