using FSH.WebApi.Shared.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Notifications;

public class NotificationPublisher : INotificationPublisher
{
    private readonly ILogger<NotificationPublisher> _logger;
    private readonly IPublisher _mediator;
    private readonly ISnackbar _snackBar;
    public NotificationPublisher(ILogger<NotificationPublisher> logger, IPublisher mediator,ISnackbar snackbar) =>
        (_logger, _mediator,_snackBar) = (logger, mediator,snackbar);

    public Task PublishAsync(INotificationMessage notification)
    {
        _logger.LogInformation("Publishing Notification : {notification}", notification.GetType().Name);
        BasicNotification basicNotification = notification as BasicNotification;
        if (basicNotification is not null)
        {
            _snackBar.Add(basicNotification.Message,(Severity) basicNotification.Label);
        }
        return _mediator.Publish(CreateNotificationWrapper(notification));
    }

    private INotification CreateNotificationWrapper(INotificationMessage notification) =>
        (INotification)Activator.CreateInstance(
            typeof(NotificationWrapper<>).MakeGenericType(notification.GetType()), notification)!;
}