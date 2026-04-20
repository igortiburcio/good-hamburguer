namespace GoodHamburger.Api.Src.Controllers.OrderController;

public record CreateOrUpdateOrderRequest
{
    public required string ClientName { get; set; }

    public required List<string> ProductIds { get; set; }
}

public record OrderResponse
{
    public required string Id { get; set; }

    public required string ClientName { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Discount { get; set; }

    public decimal TotalFinal { get; set; }

    public required List<OrderProductResponse> Products { get; set; }
}

public record OrderProductResponse
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public decimal Price { get; set; }

    public required string Type { get; set; }
}

public record ErrorResponse
{
    public required string Code { get; set; }

    public required string Message { get; set; }
}
