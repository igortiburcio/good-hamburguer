using System.Net.Http.Json;
using System.Text.Json;

namespace GoodHamburger.View.Services;

public interface IOrdersApiClient
{
    Task<ApiResult<List<OrderResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ApiResult<OrderResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<ApiResult<OrderResponseDto>> CreateAsync(CreateOrUpdateOrderRequestDto request, CancellationToken cancellationToken = default);

    Task<ApiResult<OrderResponseDto>> UpdateAsync(string id, CreateOrUpdateOrderRequestDto request, CancellationToken cancellationToken = default);

    Task<ApiResult<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);
}

public class OrdersApiClient(HttpClient httpClient) : IOrdersApiClient
{
    public async Task<ApiResult<List<OrderResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync("/api/orders", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ReadError<List<OrderResponseDto>>(response, cancellationToken);
        }

        var data = await response.Content.ReadFromJsonAsync<List<OrderResponseDto>>(cancellationToken: cancellationToken) ?? [];
        return new ApiResult<List<OrderResponseDto>>(data, null, null, (int)response.StatusCode);
    }

    public async Task<ApiResult<OrderResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"/api/orders/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ReadError<OrderResponseDto>(response, cancellationToken);
        }

        var data = await response.Content.ReadFromJsonAsync<OrderResponseDto>(cancellationToken: cancellationToken);
        return new ApiResult<OrderResponseDto>(data, null, null, (int)response.StatusCode);
    }

    public async Task<ApiResult<OrderResponseDto>> CreateAsync(CreateOrUpdateOrderRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/orders", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ReadError<OrderResponseDto>(response, cancellationToken);
        }

        var data = await response.Content.ReadFromJsonAsync<OrderResponseDto>(cancellationToken: cancellationToken);
        return new ApiResult<OrderResponseDto>(data, null, null, (int)response.StatusCode);
    }

    public async Task<ApiResult<OrderResponseDto>> UpdateAsync(string id, CreateOrUpdateOrderRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/api/orders/{id}", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ReadError<OrderResponseDto>(response, cancellationToken);
        }

        var data = await response.Content.ReadFromJsonAsync<OrderResponseDto>(cancellationToken: cancellationToken);
        return new ApiResult<OrderResponseDto>(data, null, null, (int)response.StatusCode);
    }

    public async Task<ApiResult<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"/api/orders/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ReadError<bool>(response, cancellationToken);
        }

        return new ApiResult<bool>(true, null, null, (int)response.StatusCode);
    }

    private static async Task<ApiResult<T>> ReadError<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>(cancellationToken: cancellationToken);

            if (error is not null)
            {
                return new ApiResult<T>(default, error.Code, error.Message, (int)response.StatusCode);
            }
        }
        catch (JsonException)
        {
        }

        return new ApiResult<T>(default, "UNKNOWN_ERROR", $"Erro HTTP {(int)response.StatusCode}", (int)response.StatusCode);
    }
}
