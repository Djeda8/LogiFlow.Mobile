namespace LogiFlow.Mobile.Models.AI;

/// <summary>
/// Represents a single message in the AI chat conversation.
/// </summary>
public class ChatMessage
{
    /// <summary>
    /// Gets the sender role. Valid values are "user" or "assistant".
    /// </summary>
    public string Role { get; init; } = string.Empty;

    /// <summary>
    /// Gets the message content.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Gets the UTC timestamp when the message was created.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a user message.
    /// </summary>
    /// <param name="content">The message content.</param>
    /// <returns>A user message.</returns>
    public static ChatMessage FromUser(string content) =>
        new() { Role = "user", Content = content };

    /// <summary>
    /// Creates an assistant message.
    /// </summary>
    /// <param name="content">The message content.</param>
    /// <returns>An assistant message.</returns>
    public static ChatMessage FromAssistant(string content) =>
        new() { Role = "assistant", Content = content };
}
