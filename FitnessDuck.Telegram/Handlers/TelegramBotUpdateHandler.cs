using FitnessDuck.Auth.Interfaces;
using FitnessDuck.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FitnessDuck.TelegramBot.Implementations;

public partial class TelegramBotService
{
    


    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope()) {
          var  _userService = scope.ServiceProvider.GetRequiredService<IUserService>();
          var  _tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();


        
        
        if (update.Type == UpdateType.Message)
        {
            var chatId = update.Message!.Chat.Id.ToString();
            var messageText = update.Message.Text ?? "No text";

            
            if (messageText.StartsWith("/start"))
            {
                // Extract payload following /confirm (e.g. "/confirm abc123" -> "abc123")
                var parts = messageText.Split(' ', 2);
                if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
                {
                    await botClient.SendMessage(chatId, "Please provide a confirmation code after /confirm command.", cancellationToken: cancellationToken);
                    return;
                }

         
                var confirmationCode = parts[1].Trim();

                // TODO: Validate the confirmationCode (e.g., lookup in DB, check expiry)

                var user= await _userService.GetByTelegramIdAsync(confirmationCode);
                
                if (user is null)
                    await botClient.SendMessage(chatId, $"The code you provided is invalid", cancellationToken: cancellationToken);


                if (user.TelegramConfirmed)
                    await botClient.SendMessage(chatId, $"Ehi {user.Name}, you already activated your telegram!", cancellationToken: cancellationToken);

                user=await _userService.ConfirmContactMethod(ContactMethod.Telegram, user, chatId);
                

                if (user.TelegramConfirmed)
                {
                    // Perform confirmation logic (mark user as confirmed, etc.)
                    await botClient.SendMessage(chatId, $"Confirmation successful! Code: {confirmationCode}", cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendMessage(chatId, "Invalid or expired confirmation code.", cancellationToken: cancellationToken);
                }
            }
            else
            {
                // Handle other messages or default behavior
                await botClient.SendMessage(chatId, $"Received your message: {messageText}", cancellationToken: cancellationToken);
            }
            
            
        }
        }
    }

}