using GoodHamburger.Application.Src.Errors;
using GoodHamburger.Application.Src.UseCases;
using GoodHamburger.Domain.Src;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Src.Controllers.OrderController;

[ApiController]
[Route("api/orders")]
public class OrderController(OrderUseCase orderUseCase) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrUpdateOrderRequest request)
    {
        try
        {
            var order = await orderUseCase.CreateAsync(request.ClientName, request.ProductIds);
            var response = MapOrder(order);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (DuplicateOrderItemsException ex)
        {
            return BadRequest(new ErrorResponse { Code = "DUPLICATE_ORDER_ITEMS", Message = ex.Message });
        }
        catch (InvalidOrderException ex)
        {
            return BadRequest(new ErrorResponse { Code = "INVALID_ORDER", Message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await orderUseCase.GetAllAsync();
        var response = orders.Select(MapOrder).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var order = await orderUseCase.GetByIdAsync(id);
            return Ok(MapOrder(order));
        }
        catch (ResourceNotFoundException ex)
        {
            return NotFound(new ErrorResponse { Code = "RESOURCE_NOT_FOUND", Message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateOrUpdateOrderRequest request)
    {
        try
        {
            var order = await orderUseCase.UpdateAsync(id, request.ClientName, request.ProductIds);
            return Ok(MapOrder(order));
        }
        catch (DuplicateOrderItemsException ex)
        {
            return BadRequest(new ErrorResponse { Code = "DUPLICATE_ORDER_ITEMS", Message = ex.Message });
        }
        catch (InvalidOrderException ex)
        {
            return BadRequest(new ErrorResponse { Code = "INVALID_ORDER", Message = ex.Message });
        }
        catch (ResourceNotFoundException ex)
        {
            return NotFound(new ErrorResponse { Code = "RESOURCE_NOT_FOUND", Message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await orderUseCase.DeleteAsync(id);
            return NoContent();
        }
        catch (ResourceNotFoundException ex)
        {
            return NotFound(new ErrorResponse { Code = "RESOURCE_NOT_FOUND", Message = ex.Message });
        }
    }

    private static OrderResponse MapOrder(OrderWithTotals order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            ClientName = order.ClientName,
            Subtotal = order.Subtotal,
            Discount = order.Discount,
            TotalFinal = order.TotalFinal,
            Products = order.Products.Select(MapProduct).ToList()
        };
    }

    private static OrderProductResponse MapProduct(Product product)
    {
        return new OrderProductResponse
        {
            Id = product.id,
            Name = product.Name,
            Price = product.Price,
            Category = product.Category
        };
    }
}
