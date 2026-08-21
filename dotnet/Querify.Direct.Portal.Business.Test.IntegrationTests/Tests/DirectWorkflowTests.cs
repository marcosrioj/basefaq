using Microsoft.EntityFrameworkCore;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Direct.Portal.Business.Contact.Commands.CreateContact;
using Querify.Direct.Portal.Business.Contact.Queries.GetContact;
using Querify.Direct.Portal.Business.ConversationMessage.Commands.CreateConversationMessage;
using Querify.Direct.Portal.Business.Test.IntegrationTests.Helpers;
using Querify.Models.Direct.Dtos.Contact;
using Querify.Models.Direct.Dtos.ConversationMessage;
using Querify.Models.Direct.Enums;
using Xunit;
using ContactEntity = Querify.Direct.Common.Domain.Entities.Contact;
using ConversationEntity = Querify.Direct.Common.Domain.Entities.Conversation;

namespace Querify.Direct.Portal.Business.Test.IntegrationTests.Tests;

public class DirectWorkflowTests
{
    [Fact]
    public async Task CreateAndGetContact_RoundTripsEveryContactField()
    {
        using var context = TestContext.Create();
        var createHandler = new ContactsCreateContactCommandHandler(context.DbContext, context.SessionService);
        var request = new ContactCreateRequestDto
        {
            GivenName = "Avery",
            Surname = "Morgan",
            Email = "avery.morgan@example.test",
            PhotoUrl = "https://cdn.example.test/contacts/avery.jpg",
            TimeZone = "America/Vancouver",
            PhoneNumber = "+1-555-0100",
            InstagramProfileUrl = "https://instagram.com/avery",
            TikTokProfileUrl = "https://tiktok.com/@avery",
            FacebookProfileUrl = "https://facebook.com/avery",
            SnapchatProfileUrl = "https://snapchat.com/add/avery"
        };

        var id = await createHandler.Handle(
            new ContactsCreateContactCommand { Request = request },
            CancellationToken.None);

        context.DbContext.ChangeTracker.Clear();
        var getHandler = new ContactsGetContactQueryHandler(context.DbContext, context.SessionService);
        var result = await getHandler.Handle(
            new ContactsGetContactQuery { Id = id },
            CancellationToken.None);

        Assert.Equal(context.SessionService.TenantId, result.TenantId);
        Assert.Equal(request.GivenName, result.GivenName);
        Assert.Equal(request.Surname, result.Surname);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.PhotoUrl, result.PhotoUrl);
        Assert.Equal(request.TimeZone, result.TimeZone);
        Assert.Equal(request.PhoneNumber, result.PhoneNumber);
        Assert.Equal(request.InstagramProfileUrl, result.InstagramProfileUrl);
        Assert.Equal(request.TikTokProfileUrl, result.TikTokProfileUrl);
        Assert.Equal(request.FacebookProfileUrl, result.FacebookProfileUrl);
        Assert.Equal(request.SnapchatProfileUrl, result.SnapchatProfileUrl);
        Assert.Equal(0, result.ConversationCount);
        Assert.NotNull(result.CreatedAtUtc);
        Assert.Equal(result.CreatedAtUtc, result.LastUpdatedAtUtc);
    }

    [Fact]
    public async Task CreateMessage_RejectsClosedConversation()
    {
        using var context = TestContext.Create();
        var conversation = await SeedConversationAsync(context, ConversationStatus.Closed);
        var handler = new ConversationMessagesCreateCommandHandler(context.DbContext, context.SessionService);

        var exception = await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(
            new ConversationMessagesCreateCommand
            {
                Request = new ConversationMessageCreateRequestDto
                {
                    ConversationId = conversation.Id,
                    ActorKind = MessageActorKind.User,
                    Body = "This message must not be appended.",
                    SentAtUtc = DateTime.UtcNow
                }
            },
            CancellationToken.None));

        Assert.Equal(422, exception.ErrorCode);
        Assert.Empty(await context.DbContext.ConversationMessages.AsNoTracking().ToListAsync());
    }

    [Fact]
    public async Task SaveConversation_RejectsContactFromAnotherTenant()
    {
        using var context = TestContext.Create();
        var contact = await SeedContactAsync(context);
        var conversation = new ConversationEntity
        {
            TenantId = Guid.NewGuid(),
            ContactId = contact.Id,
            Contact = contact,
            ChannelConnectionId = Guid.NewGuid(),
            Status = ConversationStatus.Open
        };
        context.DbContext.Conversations.Add(conversation);

        await Assert.ThrowsAsync<ApiErrorException>(() => context.DbContext.SaveChangesAsync());
    }

    private static async Task<ConversationEntity> SeedConversationAsync(
        TestContext context,
        ConversationStatus status)
    {
        var contact = await SeedContactAsync(context);
        var conversation = new ConversationEntity
        {
            TenantId = context.SessionService.TenantId,
            ContactId = contact.Id,
            Contact = contact,
            ChannelConnectionId = Guid.NewGuid(),
            Subject = "Billing assistance",
            Status = status
        };
        context.DbContext.Conversations.Add(conversation);
        await context.DbContext.SaveChangesAsync();
        return conversation;
    }

    private static async Task<ContactEntity> SeedContactAsync(TestContext context)
    {
        var contact = new ContactEntity
        {
            TenantId = context.SessionService.TenantId,
            GivenName = "Jordan",
            Email = "jordan@example.test"
        };
        context.DbContext.Contacts.Add(contact);
        await context.DbContext.SaveChangesAsync();
        return contact;
    }
}
