using System.Net.Http.Json;

namespace GoodHamburger.View.Services;

public interface IMenuApiClient
{
    Task<List<MenuItemDto>> GetMenuAsync(CancellationToken cancellationToken = default);
}

public class MenuApiClient(HttpClient httpClient) : IMenuApiClient
{
    public async Task<List<MenuItemDto>> GetMenuAsync(CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<List<MenuItemDto>>("/api/menu", cancellationToken);
        return result ?? [];
    }
}
