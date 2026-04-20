using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infra.Src.Db.Entities;

[Table("products")]
[Index(nameof(Name), IsUnique = true)]
[Index(nameof(CategoryId))]
public class ProductEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(120)]
    public required string Name { get; set; }

    [Precision(10, 2)]
    public decimal Price { get; set; }

    [Required]
    [Column("category_id")]
    public Guid CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public ProductCategoryEntity Category { get; set; } = null!;

    public ICollection<OrderProductEntity> OrderProducts { get; set; } = [];
}
