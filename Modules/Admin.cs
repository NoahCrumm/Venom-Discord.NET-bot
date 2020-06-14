using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Channels;

namespace VenomBot.Modules
{
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    public class AdminModule : ModuleBase
    {
        [Name("Moderator")]
        [RequireContext(ContextType.Guild)]

        [Command("purge")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]

        public async Task Purge(int amount)
        {
                // Check if the amount provided by the user is positive.
                if (amount <= 0)
                {
                    await ReplyAsync("The amount of messages to remove must be positive.");
                    return;
                }

                // Download X messages starting from Context.Message, which means
                // that it won't delete the message used to invoke this command.
                var messages = await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, amount).FlattenAsync();

                // Ensure that the messages aren't older than 14 days,
                // because trying to bulk delete messages older than that
                // will result in a bad request.
                var filteredMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);

                // Get the total amount of messages.
                var count = filteredMessages.Count();

                // Check if there are any messages to delete.
                if (count == 0)
                    await ReplyAsync("Nothing to delete.");

                else
                {
                await (Context.Channel as ITextChannel).DeleteMessagesAsync(filteredMessages);
                EmbedBuilder builder = new EmbedBuilder();

                builder.WithTitle($"Done. Removed {count} {(count > 1 ? "messages" : "message")}.");
                builder.WithCurrentTimestamp();

                builder.WithColor(Color.Red);
                await ReplyAsync("", false, builder.Build());
                }
        }


        [Command("ban")]
        [Summary("Ban's the specified user")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]

        public async Task Ban([Remainder] SocketGuildUser user)
        {

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"User {user.Mention} was banned.");
            builder.WithCurrentTimestamp();

            builder.WithColor(Color.Red);
            await ReplyAsync("", false, builder.Build());
            await user.SendMessageAsync($"You were banned from {Context.Guild}");
            await ReplyAsync($"{user.Mention}");
            await Context.Guild.AddBanAsync(user);
        }



        [Command("kick")]
        [Summary("Kick the specified user.")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick([Remainder] SocketGuildUser user)
        {
            await ReplyAsync($"User {user.Mention} kicked :wave:");
            await user.KickAsync();
        }
    }
}