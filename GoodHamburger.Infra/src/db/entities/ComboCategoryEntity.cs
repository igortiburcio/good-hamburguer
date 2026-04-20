using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infra.Src.Db.Entities;

[Table("combo_categories")]
[PrimaryKey(nameof(ComboId), nameof(CategoryId))]
public class ComboCategoryEntity
{
    [Column("combo_id")]
    public Guid ComboId { get; set; }

    [Column("category_id")]
    public Guid CategoryId { get; set; }

    [ForeignKey(nameof(ComboId))]
    public ComboEntity Combo { get; set; } = null!;

    [ForeignKey(nameof(CategoryId))]
    public ProductCategoryEntity Category { get; set; } = null!;
}
