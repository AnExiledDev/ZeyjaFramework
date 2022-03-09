using System;
using DSharpPlus;
using FluentMigrator;

namespace ZeyjaFramework.DatabaseDefinitions.Migrations
{
    /// <summary>
    /// Creates the message cache table, cache_messages.
    /// </summary>
    [Migration(20212708_1)]
    public class CreateMessageCacheTable : Migration
    {
        public override void Up()
        {
            Create.Table("cache_messages")
                .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
                .WithColumn("guild_id").AsInt64().NotNullable()
                .WithColumn("member_id").AsInt64().NotNullable()
                .WithColumn("channel_id").AsInt64().NotNullable()
                .WithColumn("message_id").AsInt64().NotNullable().Unique()
                .WithColumn("content").AsString(2500).NotNullable()
                .WithColumn("message_type").AsString(75).WithDefaultValue(MessageType.Default)
                .WithColumn("webhook_id").AsInt64().Nullable().Unique()
                .WithColumn("is_tts").AsBoolean().WithDefaultValue(false)
                .WithColumn("mentions_everyone").AsBoolean().WithDefaultValue(false)
                .WithColumn("is_pinned").AsBoolean().WithDefaultValue(false)
                .WithColumn("is_webhook").AsBoolean().WithDefaultValue(false)
                .WithColumn("created_timestamp").AsDateTime().NotNullable()
                .WithColumn("edited_timestamp").AsDateTime().WithDefaultValue(DateTime.MinValue)
                .WithColumn("deleted_timestamp").AsDateTime().WithDefaultValue(DateTime.MinValue);
        }

        public override void Down()
        {
            Delete.Table("cache_messages");
        }
    }
}
