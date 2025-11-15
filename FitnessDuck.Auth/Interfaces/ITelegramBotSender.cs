namespace FitnessDuck.Auth.Interfaces;

public interface ITelegramBotSender
{
    Task SendTokenAsync(string telegramChatId, string token);
}