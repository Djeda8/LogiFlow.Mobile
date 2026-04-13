using LogiFlow.Mobile.Models.AI;

namespace LogiFlow.Mobile.Selectors;

/// <summary>
/// Selects the appropriate template based on the message role ("user" or "assistant").
/// </summary>
public class ChatMessageTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// Gets or sets the template for user messages.
    /// </summary>
    public DataTemplate UserTemplate { get; set; } = new();

    /// <summary>
    /// Gets or sets the template for assistant messages.
    /// </summary>
    public DataTemplate AssistantTemplate { get; set; } = new();

    /// <summary>
    /// Returns the template corresponding to the message role.
    /// </summary>
    /// <param name="item">The item to evaluate.</param>
    /// <param name="container">The container.</param>
    /// <returns>The selected template.</returns>
    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is ChatMessage message)
        {
            return message.Role == "user" ? UserTemplate : AssistantTemplate;
        }

        return AssistantTemplate;
    }
}
