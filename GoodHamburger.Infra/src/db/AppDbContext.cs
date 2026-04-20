using Microsoft.EntityFrameworkCore;
using GoodHamburger.Infra.Src.Db.Entities;

namespace GoodHamburger.Infra.Src.Db;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ProductCategoryEntity> ProductCategories => Set<ProductCategoryEntity>();

    public DbSet<ProductEntity> Products => Set<ProductEntity>();

    public DbSet<ComboEntity> Combos => Set<ComboEntity>();

    public DbSet<ComboCategoryEntity> ComboCategories => Set<ComboCategoryEntity>();

    public DbSet<OrderEntity> Orders => Set<OrderEntity>();

    public DbSet<OrderProductEntity> OrderProducts => Set<OrderProductEntity>();
}
