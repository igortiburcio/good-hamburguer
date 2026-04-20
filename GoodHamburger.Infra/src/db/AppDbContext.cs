using Microsoft.EntityFrameworkCore;
using GoodHamburger.Infra.Src.Db.Entities;

namespace GoodHamburger.Infra.Src.Db;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
}
