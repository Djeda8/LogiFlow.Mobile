using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using LogiFlow.Mobile.Models.AI;
using LogiFlow.Mobile.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Calls the Anthropic Claude API (claude-haiku) to provide contextual WMS help.
/// API key is read from IConfiguration (appsettings.json / CI-injected).
/// </summary>
public sealed class ClaudeService : IClaudeService, IDisposable
{
    // ── Constants ────────────────────────────────────────────────────────────
    private const string AnthropicApiUrl = "https://api.anthropic.com/v1/messages";
    private const string AnthropicVersion = "2023-06-01";
    private const string Model = "claude-haiku-4-5-20251001";
    private const int MaxTokens = 1024;

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
    };

    // ── Fields ───────────────────────────────────────────────────────────────
    private readonly HttpClient _http;
    private readonly ILogService _log;
    private readonly string _apiKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaudeService"/> class,
    /// configuring the HTTP client with the API key from configuration.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="log">The logging service.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the "Anthropic:ApiKey" is missing from the configuration.
    /// </exception>
    public ClaudeService(IConfiguration configuration, ILogService log)
    {
        _log = log;
        _apiKey = configuration["Anthropic:ApiKey"]
                  ?? throw new InvalidOperationException(
                      "Anthropic:ApiKey is missing from configuration. " +
                      "Add it to appsettings.Development.json (local) or inject it via GitHub Actions secret (CI).");

        _http = new HttpClient();
        _http.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        _http.DefaultRequestHeaders.Add("anthropic-version", AnthropicVersion);
    }

    /// <inheritdoc/>
    public async Task<string> SendAsync(
        IReadOnlyList<ChatMessage> history,
        WmsScreenContext context,
        CancellationToken cancellationToken = default)
    {
        var systemPrompt = BuildSystemPrompt(context);
        var messages = BuildMessages(history);

        var requestBody = new ClaudeRequest
        {
            Model = Model,
            MaxTokens = MaxTokens,
            System = systemPrompt,
            Messages = messages,
        };

        _log.Info($"[ClaudeService] Sending {messages.Count} message(s) — screen: {context.ScreenId}");

        try
        {
            var response = await _http.PostAsJsonAsync(
                AnthropicApiUrl, requestBody, _jsonOpts, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _log.Info($"[ClaudeService] API error {response.StatusCode}: {errorBody}");
                throw new HttpRequestException(
                    $"Anthropic API returned {response.StatusCode}: {errorBody}");
            }

            var result = await response.Content
                .ReadFromJsonAsync<ClaudeResponse>(_jsonOpts, cancellationToken)
                ?? throw new InvalidOperationException("Empty response from Anthropic API.");

            var reply = result.Content?.FirstOrDefault()?.Text ?? string.Empty;
            _log.Info($"[ClaudeService] Reply received ({reply.Length} chars).");
            return reply;
        }
        catch (OperationCanceledException)
        {
            _log.Info("[ClaudeService] Request cancelled by user.");
            throw;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _log.Info($"[ClaudeService] Unexpected error: {ex.Message}");
            throw;
        }
    }

    // ── IDisposable ───────────────────────────────────────────────────────────

    /// <summary>
    /// Releases the resources used by the <see cref="ClaudeService"/> instance.
    /// </summary>
    public void Dispose() => _http.Dispose();

    // ── Helpers ───────────────────────────────────────────────────────────────
    private static string BuildSystemPrompt(WmsScreenContext ctx)
    {
        var state = string.IsNullOrWhiteSpace(ctx.ScreenState)
            ? "No additional state available."
            : ctx.ScreenState;

        return $"""
                You are a helpful assistant embedded in LogiFlow Mobile, a Warehouse Management System (WMS).
                Your job is to guide warehouse operators step by step through their daily tasks.

                ## Current context
                - Module       : {ctx.Module}
                - Screen       : {ctx.ScreenDisplayName} ({ctx.ScreenId})
                - Screen state : {state}

                ## Rules
                - Answer ONLY questions related to WMS operations (receiving, scanning, stock, etc.).
                - Be concise — operators are on a warehouse floor and need quick answers.
                - If you don't know something, say so clearly. Never invent WMS data.
                - Respond in the same language the operator uses.
                """;
    }

    private static List<ClaudeMessageDto> BuildMessages(IReadOnlyList<ChatMessage> history)
        => history
            .Select(m => new ClaudeMessageDto { Role = m.Role, Content = m.Content })
            .ToList();

    // ── DTOs (private, only used for JSON serialisation) ──────────────────────
    private sealed class ClaudeRequest
    {
        public string Model { get; init; } = string.Empty;

        public int MaxTokens { get; init; }

        public string System { get; init; } = string.Empty;

        public List<ClaudeMessageDto> Messages { get; init; } = [];
    }

    private sealed class ClaudeMessageDto
    {
        public string Role { get; init; } = string.Empty;

        public string Content { get; init; } = string.Empty;
    }

    private sealed class ClaudeResponse
    {
        public List<ClaudeContentBlock>? Content { get; init; }
    }

    private sealed class ClaudeContentBlock
    {
        public string? Text { get; init; }
    }
}
