using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infra.Src.Db.Entities;

[Table("orders")]
public class OrderEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(120)]
    [Column("client_name")]
    public required string ClientName { get; set; }

    [Precision(10, 2)]
    [Column("total_price")]
    public decimal TotalPrice { get; set; }

    public ICollection<OrderProductEntity> OrderProducts { get; set; } = [];
}
