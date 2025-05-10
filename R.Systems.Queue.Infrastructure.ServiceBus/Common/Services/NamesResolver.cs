using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

internal interface INamesResolver
{
    string ResolveQueueName(IQueueOptions options);
    string ResolveTopicName(ITopicOptions options);
    string ResolveSubscriptionName(ITopicOptions options);
}

internal class NamesResolver
    : INamesResolver
{
    private const int QueueNameMaxLength = 260;
    private const int TopicNameMaxLength = 260;
    private const int TopicSubscriptionNameMaxLength = 50;
    private readonly ILogger<INamesResolver> _logger;

    public NamesResolver(ILogger<NamesResolver> logger)
    {
        _logger = logger;
    }

    public string ResolveQueueName(IQueueOptions options)
    {
        string queueName = options.UseMachineNameAsQueueSuffix ? ApplySuffix(options.QueueName) : options.QueueName;
        if (queueName.Length <= QueueNameMaxLength)
        {
            return queueName;
        }

        _logger.LogWarning(
            "Queue name '{QueueName}' exceeds the maximum length of {MaxLength}. It will be truncated",
            queueName,
            QueueNameMaxLength
        );

        return queueName[..QueueNameMaxLength];
    }

    public string ResolveTopicName(ITopicOptions options)
    {
        string topicName = options.UseMachineNameAsTopicSuffix ? ApplySuffix(options.TopicName) : options.TopicName;
        if (topicName.Length <= TopicNameMaxLength)
        {
            return topicName;
        }

        _logger.LogWarning(
            "Topic name '{TopicName}' exceeds the maximum length of {MaxLength}. It will be truncated",
            topicName,
            TopicNameMaxLength
        );

        return topicName[..TopicNameMaxLength];
    }

    public string ResolveSubscriptionName(ITopicOptions options)
    {
        string subscriptionName = options.UseMachineNameAsSubscriptionSuffix
            ? ApplySuffix(options.SubscriptionName)
            : options.SubscriptionName;
        if (subscriptionName.Length <= TopicSubscriptionNameMaxLength)
        {
            return subscriptionName;
        }

        _logger.LogWarning(
            "Subscription name '{SubscriptionName}' exceeds the maximum length of {MaxLength}. It will be truncated",
            subscriptionName,
            TopicSubscriptionNameMaxLength
        );

        return subscriptionName[..TopicSubscriptionNameMaxLength];
    }

    private string ApplySuffix(string name)
    {
        string? suffix = GetSuffix();
        string nameWithSuffix = suffix is null ? name : $"{name}-{suffix}";

        return SanitizeName(nameWithSuffix);
    }

    private string? GetSuffix()
    {
        string? websiteInstanceId =
            Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")?.ToLowerInvariant()?.Trim();
        if (!string.IsNullOrEmpty(websiteInstanceId))
        {
            return websiteInstanceId;
        }

        return Environment.MachineName?.ToLowerInvariant()?.Trim();
    }

    private string SanitizeName(string name)
    {
        return Regex.Replace(name, @"[^a-zA-Z0-9\-]", "");
    }
}
