using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.Persistence.EntityConfiguration;

class MessageEntityConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.UserName);
        builder.Property(e => e.Text);
        builder.Property(e => e.SentAt)
        .HasConversion
        (
            src => src.HasValue ? src.Value.ToUniversalTime() : src,
            dst => dst.HasValue ? dst.Value.ToUniversalTime() : dst
        );
        builder.Property(e => e.EditedAt)
        .HasConversion
        (
            src => src.HasValue ? src.Value.ToUniversalTime() : src,
            dst => dst.HasValue ? dst.Value.ToUniversalTime() : dst
        );
    }
}
