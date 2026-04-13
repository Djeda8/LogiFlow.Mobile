using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogiFlow.Mobile.Models.AI;
using LogiFlow.Mobile.Resources.Languages;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Base;

namespace LogiFlow.Mobile.ViewModels.AI;

/// <summary>
/// Manages the AI chat bottom sheet state.
/// Registered as Transient — one instance per chat session.
/// The owning page must call <see cref="InitialiseContext"/> before opening the sheet.
/// </summary>
public partial class ChatViewModel : BaseViewModel
{
    // ── Dependencies ──────────────────────────────────────────────────────────
    private readonly IClaudeService _claudeService;
    private readonly ILogService _logService;
    private readonly ILocalizationService _localizationService;

    // ── State ─────────────────────────────────────────────────────────────────
    private WmsScreenContext _context = new();
    private CancellationTokenSource? _cts;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private bool _isThinking;

    // ── Constructor ───────────────────────────────────────────────────────────

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatViewModel"/> class
    /// with the specified services for AI interaction, logging, and localization.
    /// </summary>
    /// <param name="claudeService">The AI chat service.</param>
    /// <param name="logService">The logging service.</param>
    /// <param name="localizationService">The localization service.</param>
    public ChatViewModel(
        IClaudeService claudeService,
        ILogService logService,
        ILocalizationService localizationService)
    {
        _claudeService = claudeService;
        _logService = logService;
        _localizationService = localizationService;
    }

    // ── Observable properties ─────────────────────────────────────────────────

    /// <summary>
    /// Gets the collection of chat messages.
    /// </summary>
    public ObservableCollection<ChatMessage> Messages { get; } = [];

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Initializes the chat context with the given screen information,
    /// clears existing messages, resets input, and adds a localized greeting.
    /// </summary>
    /// <param name="context">The current WMS screen context.</param>
    public void InitialiseContext(WmsScreenContext context)
    {
        _context = context;
        Messages.Clear();
        InputText = string.Empty;
        HasError = false;

        // Greeting localizado — usa {0} para el nombre de pantalla
        var greetingTemplate = _localizationService.GetString(nameof(AppResources.AiChatGreeting));
        var greeting = string.Format(greetingTemplate, context.ScreenDisplayName);

        Messages.Add(ChatMessage.FromAssistant(greeting));
        _logService.Info($"[ChatViewModel] Context initialised — screen: {context.ScreenId}");
    }

    /// <summary>Cancels the in-flight request (e.g. user closes the sheet).</summary>
    public void CancelPendingRequest()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        IsThinking = false;
    }

    // ── Commands ──────────────────────────────────────────────────────────────
    private bool CanSend() => !IsBusy && !string.IsNullOrWhiteSpace(InputText);

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task SendAsync()
    {
        var userText = InputText.Trim();
        InputText = string.Empty;

        Messages.Add(ChatMessage.FromUser(userText));
        HasError = false;
        IsBusy = true;
        IsThinking = true;

        _cts = new CancellationTokenSource();

        try
        {
            var history = (IReadOnlyList<ChatMessage>)Messages.ToList();
            var reply = await _claudeService.SendAsync(history, _context, _cts.Token);

            Messages.Add(ChatMessage.FromAssistant(reply));
            _logService.Info("[ChatViewModel] Assistant reply added to history.");
        }
        catch (OperationCanceledException)
        {
            _logService.Info("[ChatViewModel] Send cancelled.");
        }
        catch (Exception ex)
        {
            ErrorMessage = _localizationService.GetString(nameof(AppResources.AiChatErrorService));
            HasError = true;
            _logService.Info($"[ChatViewModel] Error during send: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            IsThinking = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    [RelayCommand]
    private void ClearChat()
    {
        CancelPendingRequest();
        InitialiseContext(_context);
    }
}
