using GoodHamburger.Domain.Src;

namespace GoodHamburger.Application.Src.Repositories;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);

    Task<Order?> GetByIdAsync(string id);

    Task<List<Order>> GetAllAsync();

    Task<Order?> UpdateAsync(Order order);

    Task<bool> DeleteAsync(string id);
}
