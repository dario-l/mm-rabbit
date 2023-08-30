using System;

namespace Fast.Shared.Abstractions.Messaging;

[AttributeUsage(AttributeTargets.Class)]
public class MessageAttribute : Attribute
{
    public string Topic { get; }
    public string Exchange { get; }
    public string Queue { get; }
    public string QueueType { get; }
    public string ErrorQueue { get; }
    public string SubscriptionId { get; }

    public MessageAttribute(string? topic = null, string? exchange = null, string? queue = null,
        string? queueType = null, string? errorQueue = null, string? subscriptionId = null)
    {
        Topic = topic ?? string.Empty;
        Exchange = exchange ?? string.Empty;
        Queue = queue ?? string.Empty;
        QueueType = queueType ?? string.Empty;
        ErrorQueue = errorQueue ?? string.Empty;
        SubscriptionId = subscriptionId ?? string.Empty;
    }
}