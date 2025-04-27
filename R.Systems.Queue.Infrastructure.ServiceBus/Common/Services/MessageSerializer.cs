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
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly ILogger<MessageSerializer> _logger;

    public MessageSerializer(ILogger<MessageSerializer> logger)
    {
        _logger = logger;
    }

    public TMessage Deserialize<TMessage>(string message)
    {
        try
        {
            TMessage? deserializedMessage = JsonSerializer.Deserialize<TMessage>(message, Options);
            if (deserializedMessage is null)
            {
                throw new JsonException("Deserialized object is null.");
            }

            return deserializedMessage;
        }
        catch (JsonException jsonException)
        {
            _logger.LogError(jsonException, "An unexpected error in deserializing message has occurred");

            throw;
        }
    }

    public string Serialize<TMessage>(TMessage message)
    {
        return JsonSerializer.Serialize(message, Options);
    }
}
