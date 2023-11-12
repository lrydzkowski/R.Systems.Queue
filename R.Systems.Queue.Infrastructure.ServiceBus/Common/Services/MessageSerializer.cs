using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

public interface IMessageSerializer
{
    TMessage Deserialize<TMessage>(string message);
    string Serialize<TMessage>(TMessage message);
}

public class MessageSerializer : IMessageSerializer
{
    public MessageSerializer(ILogger<MessageSerializer> logger)
    {
        Logger = logger;
    }

    private ILogger<MessageSerializer> Logger { get; }

    public TMessage Deserialize<TMessage>(string message)
    {
        try
        {
            TMessage? deserializedMessage = JsonSerializer.Deserialize<TMessage>(message);
            if (deserializedMessage is null)
            {
                throw new JsonException("Deserialized object is null.");
            }

            return deserializedMessage;
        }
        catch (JsonException jsonException)
        {
            Logger.LogError(jsonException, "An unexpected error in deserializing message has occurred.");

            throw;
        }
    }

    public string Serialize<TMessage>(TMessage message)
    {
        return JsonSerializer.Serialize(message);
    }
}
