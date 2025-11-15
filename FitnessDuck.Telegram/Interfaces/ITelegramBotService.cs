namespace FitnessDuck.TelegramBot.Interfaces;

public interface ITelegramBotService
{
    Task SendMessageAsync(string chatId, string message);
    
    
   
}