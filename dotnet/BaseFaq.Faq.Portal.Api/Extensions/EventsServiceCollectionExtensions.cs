using BaseFaq.AI.Common.Contracts.Generation;
using BaseFaq.Common.Infrastructure.MassTransit.Models;
using MassTransit;

namespace BaseFaq.Faq.Portal.Api.Extensions;

public static class EventsServiceCollectionExtensions
{
    public static IServiceCollection AddEventsFeature(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqOption = configuration.GetSection(RabbitMqOption.Name).Get<RabbitMqOption>()
                             ?? throw new InvalidOperationException("RabbitMQ configuration is missing.");

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((_, cfg) =>
            {
                cfg.Host(new Uri($"rabbitmq://{rabbitMqOption.Hostname}:{rabbitMqOption.Port}/"), h =>
                {
                    h.Username(rabbitMqOption.Username);
                    h.Password(rabbitMqOption.Password);
                });

                cfg.Message<FaqGenerationRequestedV1>(message =>
                    message.SetEntityName(rabbitMqOption.Exchange.Name));

                cfg.Publish<FaqGenerationRequestedV1>(message =>
                    message.ExchangeType = rabbitMqOption.Exchange.Type);
            });
        });

        return services;
    }
}