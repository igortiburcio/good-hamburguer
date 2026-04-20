using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infra.Src.Db.Entities;

[Table("order_products")]
[PrimaryKey(nameof(OrderId), nameof(ProductId))]
public class OrderProductEntity
{
    [Column("order_id")]
    public Guid OrderId { get; set; }

    [Column("product_id")]
    public Guid ProductId { get; set; }

    [Precision(10, 2)]
    [Column("unit_price")]
    public decimal UnitPrice { get; set; }

    public OrderEntity Order { get; set; } = null!;

    public ProductEntity Product { get; set; } = null!;
}
