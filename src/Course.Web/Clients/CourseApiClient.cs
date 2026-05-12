using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Course.Web.Models;

namespace Course.Web.Clients;

public sealed class CourseApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public CourseApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<ApiResult<IReadOnlyList<ProductResponse>>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        return ExecuteAsync<IReadOnlyList<ProductResponse>>(
            token => _httpClient.GetAsync("api/products", token),
            cancellationToken);
    }

    public Task<ApiResult<OrderResponse>> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        return ExecuteAsync<OrderResponse>(
            token => _httpClient.PostAsJsonAsync("api/orders", request, JsonOptions, token),
            cancellationToken);
    }

    public Task<ApiResult<OrderResponse>> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return ExecuteAsync<OrderResponse>(
            token => _httpClient.GetAsync($"api/orders/{orderId}", token),
            cancellationToken);
    }

    public Task<ApiResult<OrderResponse>> CancelOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return ExecuteAsync<OrderResponse>(
            token => _httpClient.PostAsync($"api/orders/{orderId}/cancel", content: null, token),
            cancellationToken);
    }

    private static async Task<ApiResult<T>> ReadResultAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await ReadErrorAsync(response, cancellationToken);
            return ApiResult<T>.Failure(error, response.StatusCode);
        }

        var payload = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
        return payload is null
            ? ApiResult<T>.Failure("La API respondio sin contenido.", response.StatusCode)
            : ApiResult<T>.Ok(payload, response.StatusCode);
    }

    private static async Task<ApiResult<T>> ExecuteAsync<T>(
        Func<CancellationToken, Task<HttpResponseMessage>> request,
        CancellationToken cancellationToken)
    {
        try
        {
            using var response = await request(cancellationToken);
            return await ReadResultAsync<T>(response, cancellationToken);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return ApiResult<T>.Failure("La API no respondio antes del tiempo esperado.");
        }
        catch (HttpRequestException exception)
        {
            return ApiResult<T>.Failure($"No se pudo conectar con Course.Api. {exception.Message}");
        }
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return $"La API respondio {(int)response.StatusCode} {response.ReasonPhrase}.";
        }

        try
        {
            using var document = JsonDocument.Parse(content);
            if (document.RootElement.TryGetProperty("detail", out var detail) &&
                detail.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(detail.GetString()))
            {
                return detail.GetString()!;
            }

            if (document.RootElement.TryGetProperty("title", out var title) &&
                title.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(title.GetString()))
            {
                return title.GetString()!;
            }
        }
        catch (JsonException)
        {
            return content;
        }

        return content;
    }
}
