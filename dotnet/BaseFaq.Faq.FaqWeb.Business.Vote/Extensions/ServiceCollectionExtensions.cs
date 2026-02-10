using BaseFaq.Faq.FaqWeb.Business.Vote.Abstractions;
using BaseFaq.Faq.FaqWeb.Business.Vote.Commands.CreateVote;
using BaseFaq.Faq.FaqWeb.Business.Vote.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Faq.FaqWeb.Business.Vote.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVoteBusiness(this IServiceCollection services)
    {
        services.AddScoped<IVoteService, VoteService>();
        services.AddMediatR(config =>
            config.RegisterServicesFromAssemblyContaining<VotesCreateVoteCommandHandler>());

        return services;
    }
}