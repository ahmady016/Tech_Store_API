using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bogus;

using TechStoreApi.DB.Common;

namespace TechStoreApi.Entities;

public class ContactUsMessage : Entity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public MessageTypes TypeId { get; set; }
    public ReplyTypes ReplyTypeId { get; set; } = ReplyTypes.None;
    public string Reply { get; set; }
    public string RepliedBy { get; set; }
    public DateTime? RepliedAt { get; set; }
}

public class ContactUsMessageConfig : EntityConfig<ContactUsMessage>
{
    public override void Configure(EntityTypeBuilder<ContactUsMessage> entity)
    {
        entity.ToTable("contact_us_messages");
        base.Configure(entity);

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("name")
            .HasColumnType("nvarchar(50)");

        entity.Property(e => e.Email)
            .HasMaxLength(100)
            .HasColumnName("email")
            .HasColumnType("varchar(100)");

        entity.Property(e => e.Mobile)
            .HasMaxLength(15)
            .HasColumnName("mobile")
            .HasColumnType("varchar(15)");

        entity.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("title")
            .HasColumnType("nvarchar(100)");

        entity.Property(e => e.Content)
            .IsRequired()
            .HasMaxLength(800)
            .HasColumnName("content")
            .HasColumnType("nvarchar(800)");

        entity.Property(e => e.TypeId)
            .IsRequired()
            .HasColumnName("type_id")
            .HasColumnType("tinyint");

        entity.Property(e => e.ReplyTypeId)
            .IsRequired()
            .HasDefaultValue(ReplyTypes.None)
            .HasColumnName("reply_type_id")
            .HasColumnType("tinyint");

        entity.Property(e => e.Reply)
            .HasMaxLength(500)
            .HasColumnName("reply")
            .HasColumnType("nvarchar(500)");

        entity.Property(e => e.RepliedBy)
            .HasMaxLength(100)
            .HasColumnName("replied_by")
            .HasColumnType("varchar(100)");

        entity.Property(e => e.RepliedAt)
            .HasColumnName("replied_at")
            .HasColumnType("datetime2(3)");

    }
}

public class ContactUsMessageFaker : Faker<ContactUsMessage> {
    public ContactUsMessageFaker()
    {
        RuleFor(o => o.Name, f => f.Name.FullName(f.PickRandom<Bogus.DataSets.Name.Gender>()));
        RuleFor(o => o.Email, f => f.Internet.Email().OrNull(f, 0.4f));
        RuleFor(o => o.Mobile, f => String.Join("", f.Random.Digits(11)).OrNull(f, 0.6f));
        RuleFor(o => o.Title, f => f.Lorem.Sentence(f.Random.Int(4, 12)));
        RuleFor(o => o.Content, f => f.Lorem.Sentences(f.Random.Int(4, 10)));
        RuleFor(o => o.TypeId, f => f.PickRandom<MessageTypes>());
        RuleFor(o => o.ReplyTypeId, f => f.PickRandom<ReplyTypes>().OrDefault(f, 0.4f));
        RuleFor(o => o.Reply, f => f.Lorem.Sentences(f.Random.Int(4, 10)).OrNull(f, 0.4f));
        RuleFor(o => o.RepliedAt, f => f.Date.Between(f.Date.Past(2), DateTime.UtcNow).OrNull(f, 0.4f));
        RuleFor(o => o.RepliedBy, f => f.Internet.Email().OrNull(f, 0.4f));
    }
}
