using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infra.Src.Db.Entities;

[Table("combos")]
public class ComboEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(120)]
    public required string Name { get; set; }

    [Precision(5, 2)]
    [Column("discount_percent")]
    public decimal DiscountPercent { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    public ICollection<ComboCategoryEntity> ComboCategories { get; set; } = [];
}
