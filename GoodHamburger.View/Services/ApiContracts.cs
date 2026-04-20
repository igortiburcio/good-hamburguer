namespace GoodHamburger.View.Services;

public record MenuItemDto
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public decimal Price { get; set; }

    public required string Category { get; set; }
}

public record CreateOrUpdateOrderRequestDto
{
    public required string ClientName { get; set; }

    public required List<string> ProductIds { get; set; }
}

public record OrderResponseDto
{
    public required string Id { get; set; }

    public required string ClientName { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Discount { get; set; }

    public decimal TotalFinal { get; set; }

    public required List<OrderProductResponseDto> Products { get; set; }
}

public record OrderProductResponseDto
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public decimal Price { get; set; }

    public required string Category { get; set; }
}

public record ErrorResponseDto
{
    public required string Code { get; set; }

    public required string Message { get; set; }
}

public record ApiResult<T>(T? Data, string? ErrorCode, string? ErrorMessage, int StatusCode)
{
    public bool IsSuccess => StatusCode is >= 200 and < 300;
}
