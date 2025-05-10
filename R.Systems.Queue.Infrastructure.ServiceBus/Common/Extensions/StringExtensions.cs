namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Extensions;

internal static class StringExtensions
{
    public static string SanitizeServiceBusMessageId(this string? messageId)
    {
        string sanitized = new(messageId?.Where(char.IsLetterOrDigit).ToArray() ?? []);
        sanitized = sanitized.ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(sanitized))
        {
            return Guid.NewGuid().ToString();
        }

        return sanitized.Length > 128 ? sanitized[..128] : sanitized;
    }
}
