using GoodHamburger.View.Services;
using Microsoft.AspNetCore.Components;

namespace GoodHamburger.View.Components.Pages;

public abstract class OrderFormBase : ComponentBase
{
    [Inject]
    protected IMenuApiClient MenuApiClient { get; set; } = null!;

    [Inject]
    protected IOrdersApiClient OrdersApiClient { get; set; } = null!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = null!;

    protected readonly List<MenuItemDto> MenuItems = [];
    protected readonly Dictionary<string, string> SelectedByCategory = new(StringComparer.OrdinalIgnoreCase);
    protected bool IsLoading = true;
    protected bool IsSubmitting;
    protected string? ErrorCode;
    protected string? ErrorMessage;
    protected string ClientName = string.Empty;

    protected async Task LoadMenuAsync()
    {
        IsLoading = true;
        ErrorCode = null;
        ErrorMessage = null;

        var menu = await MenuApiClient.GetMenuAsync();

        MenuItems.Clear();
        MenuItems.AddRange(menu.OrderBy(m => m.Category).ThenBy(m => m.Name));

        foreach (var category in MenuItems.Select(m => m.Category).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            SelectedByCategory.TryAdd(category, string.Empty);
        }

        IsLoading = false;
    }

    protected IEnumerable<IGrouping<string, MenuItemDto>> GroupedMenu()
    {
        return MenuItems.GroupBy(m => m.Category)
            .OrderBy(g => g.Key);
    }

    protected List<string> SelectedProductIds()
    {
        return SelectedByCategory.Values
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    protected async Task SubmitAsync(Func<CreateOrUpdateOrderRequestDto, Task<ApiResult<OrderResponseDto>>> action)
    {
        IsSubmitting = true;
        ErrorCode = null;
        ErrorMessage = null;

        var request = new CreateOrUpdateOrderRequestDto
        {
            ClientName = ClientName,
            ProductIds = SelectedProductIds()
        };

        var result = await action(request);

        if (!result.IsSuccess)
        {
            ErrorCode = result.ErrorCode;
            ErrorMessage = result.ErrorMessage;
            IsSubmitting = false;
            return;
        }

        NavigationManager.NavigateTo("/orders");
    }
}
