using System.Net.Http.Json;
using JobTimecardTracker.WinForms.Models;

namespace JobTimecardTracker.WinForms.Services;

public class ApiClientService
{
    private readonly HttpClient _httpClient;

    public ApiClientService(string baseUrl)
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/"),
            Timeout = TimeSpan.FromSeconds(15)
        };
    }

    public async Task<List<JobDto>> GetJobsAsync()
    {
        var jobs = await _httpClient.GetFromJsonAsync<List<JobDto>>("api/jobs");
        return jobs ?? new List<JobDto>();
    }

    public async Task<JobDto> CreateJobAsync(CreateJobDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/jobs", dto);
        await EnsureSuccessWithMessage(response);
        var created = await response.Content.ReadFromJsonAsync<JobDto>();
        return created ?? throw new InvalidOperationException("Failed to deserialize created job.");
    }

    public async Task<List<TimecardDto>> GetTimecardsAsync(string? employee = null, DateTime? dateWorked = null)
    {
        var queryParts = new List<string>();
        if (!string.IsNullOrWhiteSpace(employee))
        {
            queryParts.Add($"employee={Uri.EscapeDataString(employee.Trim())}");
        }
        if (dateWorked.HasValue)
        {
            queryParts.Add($"dateWorked={dateWorked.Value:yyyy-MM-dd}");
        }

        var url = "api/timecards" + (queryParts.Count > 0 ? "?" + string.Join("&", queryParts) : "");
        var timecards = await _httpClient.GetFromJsonAsync<List<TimecardDto>>(url);
        return timecards ?? new List<TimecardDto>();
    }

    public async Task<TimecardDto> CreateTimecardAsync(CreateTimecardDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/timecards", dto);
        await EnsureSuccessWithMessage(response);
        var created = await response.Content.ReadFromJsonAsync<TimecardDto>();
        return created ?? throw new InvalidOperationException("Failed to deserialize created timecard.");
    }

    private static async Task EnsureSuccessWithMessage(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync();
        var message = string.IsNullOrWhiteSpace(body)
            ? $"API request failed with status {(int)response.StatusCode} ({response.ReasonPhrase})."
            : body;
        throw new HttpRequestException(message, null, response.StatusCode);
    }
}