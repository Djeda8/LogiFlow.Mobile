using LogiFlow.Mobile.Models.AI;

namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Sends messages to the Anthropic Claude API and returns the assistant reply.
/// </summary>
public interface IClaudeService
{
    /// <summary>
    /// Sends the conversation history plus a WMS screen context and returns
    /// the assistant's next message.
    /// </summary>
    /// <param name="history">
    /// Ordered list of previous messages (user + assistant turns).
    /// The last item must be the latest user message.
    /// </param>
    /// <param name="context">
    /// Current WMS screen context injected into the system prompt.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The assistant reply text.</returns>
    Task<string> SendAsync(
        IReadOnlyList<ChatMessage> history,
        WmsScreenContext context,
        CancellationToken cancellationToken = default);
}
