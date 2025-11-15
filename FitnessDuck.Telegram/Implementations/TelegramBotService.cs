using FitnessDuck.Auth.Interfaces;
using FitnessDuck.Exceptions;
using FitnessDuck.TelegramBot.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FitnessDuck.TelegramBot.Implementations;

public partial class TelegramBotService : ITelegramBotService
{
    private readonly TelegramBotClient _botClient;
    private readonly ILogger<TelegramBotService> _logger;
    

    private readonly IConfiguration _config;
    private readonly IServiceProvider _serviceProvider;

    private readonly string _botToken;
    // Inject bot token via constructor
    public TelegramBotService(IServiceProvider serviceProvider,  ILogger<TelegramBotService> logger,IConfiguration config)
    {
        _serviceProvider = serviceProvider;
        _config = config;
        
        var telegramSettings = _config.GetSection("Telegram");
        
        
        
        
        
        _botToken=telegramSettings["Token"] ?? throw new FitnessDuckServerException("config_missing_telegram_token","Bot's token is not configured");

        
        

        _logger = logger;
        _botClient = new TelegramBotClient(_botToken);
        
        var me = _botClient.GetMe().Result;
        Console.WriteLine($"Bot id: {me.Id}, Bot Name: {me.FirstName}");
 	
        StartReceiving();
        
        
    }

    public async Task SendMessageAsync(string chatId, string message)
    {
        if (string.IsNullOrEmpty(chatId))
            throw new ArgumentException("Chat ID must be provided", nameof(chatId));
        await _botClient.SendMessage(chatId: chatId, text: message);
    }
    
    
    private void StartReceiving()
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken: CancellationToken.None);
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Telegram bot error");
        return Task.CompletedTask;
    }
}