using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Querify.Common.EntityFramework.Core.Configurations;
using Querify.Direct.Common.Domain.Entities;

namespace Querify.Direct.Common.Persistence.DirectDb.Configurations;

public sealed class ContactConfiguration : BaseConfiguration<Contact>
{
    public override void Configure(EntityTypeBuilder<Contact> builder)
    {
        base.Configure(builder);

        builder.ToTable("Contacts");

        builder.Property(contact => contact.GivenName)
            .HasMaxLength(Contact.MaxGivenNameLength)
            .IsRequired();
        builder.Property(contact => contact.Surname).HasMaxLength(Contact.MaxSurnameLength);
        builder.Property(contact => contact.Email).HasMaxLength(Contact.MaxEmailLength);
        builder.Property(contact => contact.TimeZone).HasMaxLength(Contact.MaxTimeZoneLength);
        builder.Property(contact => contact.PhotoUrl).HasMaxLength(Contact.MaxPhotoUrlLength);
        builder.Property(contact => contact.PhoneNumber).HasMaxLength(Contact.MaxPhoneNumberLength);
        builder.Property(contact => contact.InstagramProfileUrl).HasMaxLength(Contact.MaxInstagramProfileUrlLength);
        builder.Property(contact => contact.TikTokProfileUrl).HasMaxLength(Contact.MaxTikTokProfileUrlLength);
        builder.Property(contact => contact.FacebookProfileUrl).HasMaxLength(Contact.MaxFacebookProfileUrlLength);
        builder.Property(contact => contact.SnapchatProfileUrl).HasMaxLength(Contact.MaxSnapchatProfileUrlLength);
        builder.Property(contact => contact.TenantId).IsRequired();

        builder.HasIndex(contact => new { contact.TenantId, contact.Email })
            .HasDatabaseName("IX_Contact_TenantId_Email");
        builder.HasIndex(contact => new { contact.TenantId, contact.PhoneNumber })
            .HasDatabaseName("IX_Contact_TenantId_PhoneNumber");
    }
}
