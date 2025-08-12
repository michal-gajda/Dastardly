namespace Dastardly.Infrastructure.Clients;

using Dastardly.Infrastructure.ServiceDiscovery;
using Microsoft.Extensions.Logging;
using System.Text.Json;

/// <summary>
/// Example service client that demonstrates service discovery usage
/// </summary>
public interface IExampleServiceClient
{
    /// <summary>
    /// Gets data from another service using service discovery
    /// </summary>
    Task<ExampleResponse?> GetDataAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Posts data to another service using service discovery
    /// </summary>
    Task<bool> PostDataAsync(ExampleRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Example request model
/// </summary>
public sealed record ExampleRequest
{
    public required string Id { get; init; }
    public required string Data { get; init; }
}

/// <summary>
/// Example response model
/// </summary>
public sealed record ExampleResponse
{
    public required string Id { get; init; }
    public required string Data { get; init; }
    public DateTime Timestamp { get; init; }
}

/// <summary>
/// Implementation of example service client using service discovery
/// </summary>
public sealed class ExampleServiceClient : IExampleServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExampleServiceClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ExampleServiceClient(HttpClient httpClient, ILogger<ExampleServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<ExampleResponse?> GetDataAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching data for ID {Id} from example-service", id);

            // The service name 'example-service' will be resolved by the ServiceDiscoveryHttpMessageHandler
            var response = await _httpClient.GetAsync($"http://example-service/api/data/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonSerializer.Deserialize<ExampleResponse>(content, _jsonOptions);
            }

            _logger.LogWarning("Failed to fetch data for ID {Id}. Status: {StatusCode}", id, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching data for ID {Id} from example-service", id);
            throw;
        }
    }

    public async Task<bool> PostDataAsync(ExampleRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Posting data for ID {Id} to example-service", request.Id);

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            // The service name 'example-service' will be resolved by the ServiceDiscoveryHttpMessageHandler
            var response = await _httpClient.PostAsync("http://example-service/api/data", content, cancellationToken);

            var success = response.IsSuccessStatusCode;

            if (success)
            {
                _logger.LogInformation("Successfully posted data for ID {Id} to example-service", request.Id);
            }
            else
            {
                _logger.LogWarning("Failed to post data for ID {Id}. Status: {StatusCode}", request.Id, response.StatusCode);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error posting data for ID {Id} to example-service", request.Id);
            throw;
        }
    }
}
