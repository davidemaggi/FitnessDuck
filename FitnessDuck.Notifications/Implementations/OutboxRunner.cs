using FitnessDuck.Data.Repositories.Interfaces;
using FitnessDuck.Mail.Interfaces;
using FitnessDuck.Models;
using FitnessDuck.Notifications.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FitnessDuck.Notifications.Implementations;

public class OutboxRunner: IHostedService, IDisposable
{
    private readonly ILogger<OutboxRunner> _logger;
    private readonly IEmailSender _emailSender;
    private Timer? _timer;
    private bool _isRunning;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);

    public OutboxRunner(ILogger<OutboxRunner> logger, IEmailSender emailSender, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _emailSender = emailSender;
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("OutboxRunner starting.");
        _timer = new Timer(async _ => await ExecuteJobAsync(), null, TimeSpan.Zero, _interval);
        return Task.CompletedTask;
    }

    private async Task ExecuteJobAsync()
    {
        if (_isRunning)
        {
            _logger.LogInformation("Job already running; skipping this interval.");
            return;
        }

        try
        {
            _isRunning = true;
            _logger.LogInformation("Job started.");
            // Your async job logic here
            await RunYourJobAsync();
            _logger.LogInformation("Job finished.");
        }
        catch (Exception ex)
        {
            // Log and handle exceptions appropriately
            _logger.LogError(ex, "Error occured executing the job.");
        }
        finally
        {
            _isRunning = false;
        }
    }

    private async Task RunYourJobAsync()
    {
        // Simulate work
        
        using var scope = _scopeFactory.CreateScope();
        var _outboxService = scope.ServiceProvider.GetRequiredService<INotificationOutboxService>();

        var toSend = await _outboxService.GetPendingMessages();
        var toSendList = toSend.ToList();
        if (!toSendList.Any())
            return;

        foreach (var dto in toSendList)
        {

            try
            {
           

            if (dto.Method==ContactMethod.Email)
            {
                
                _emailSender.SendEmailAsync(dto.User is not null? dto.User.Email : dto.Contact, dto.Subject??"" ,dto.Message);
                await _outboxService.MessageSent(dto.Id);

            }

            }
            catch (Exception ex)
            {
                await _outboxService.MessageFailed(dto.Id);
            }

        }

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("IntervalJobService stopping.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}