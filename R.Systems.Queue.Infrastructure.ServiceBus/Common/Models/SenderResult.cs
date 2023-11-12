namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Models;

public class SenderResult
{
    public List<string> MessageIds { get; init; } = new();
}
