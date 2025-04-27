using System.Text.RegularExpressions;
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
    public string ResolveQueueName(IQueueOptions options)
    {
        return options.UseMachineNameAsQueueSuffix ? ApplySuffix(options.QueueName) : options.QueueName;
    }

    public string ResolveTopicName(ITopicOptions options)
    {
        return options.UseMachineNameAsTopicSuffix ? ApplySuffix(options.TopicName) : options.TopicName;
    }

    public string ResolveSubscriptionName(ITopicOptions options)
    {
        return options.UseMachineNameAsSubscriptionSuffix
            ? ApplySuffix(options.SubscriptionName)
            : options.SubscriptionName;
    }

    private string ApplySuffix(string name)
    {
        string? suffix = GetSuffix();
        string nameWithSuffix = suffix is null ? name : $"{name}-{suffix}";

        return SanitizeName(nameWithSuffix);
    }

    private string? GetSuffix()
    {
        return Environment.MachineName?.ToLowerInvariant();
    }

    private string SanitizeName(string name)
    {
        return Regex.Replace(name, @"[^a-zA-Z0-9\-]", "");
    }
}
