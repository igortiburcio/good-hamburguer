using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infra.Src.Db.Entities;

[Table("product_categories")]
[Index(nameof(Name), IsUnique = true)]
public class ProductCategoryEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(60)]
    public required string Name { get; set; }

    public ICollection<ProductEntity> Products { get; set; } = [];

    public ICollection<ComboCategoryEntity> ComboCategories { get; set; } = [];
}
