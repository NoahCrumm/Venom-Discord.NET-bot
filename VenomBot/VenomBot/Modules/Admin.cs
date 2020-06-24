using Discord;
using Newtonsoft.Json;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenomBot.Modules
{
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    public class Admin : ModuleBase<SocketCommandContext>
    {

        [Command("slowmode")]
        [RequireUserPermission(GuildPermission.KickMembers)]

        public async Task Slowmode(string value = null)
        {

            if (value == null)
            {
                EmbedBuilder novalue = new EmbedBuilder();

                novalue.WithTitle("Please provide a slowmode timer in seconds!");
                novalue.WithDescription("A value is needed in order to set the channel slowmode.");
                novalue.WithCurrentTimestamp();
                novalue.WithFooter($"{Context.Message.Author.ToString()}");
                novalue.WithColor(Color.DarkPurple);

                await ReplyAsync("", false, novalue.Build());

                return;
            }

            int val = 0;
            val = Convert.ToInt32(value);
            var chan = Context.Guild.GetTextChannel(Context.Channel.Id);
            await chan.ModifyAsync(x =>
            {
                x.SlowModeInterval = val;
            });

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"Slowmode set to {value}s");
            builder.WithDescription($"Successfully set slowmode to {value}s");
            builder.WithCurrentTimestamp();
            builder.WithFooter($"{Context.Message.Author.ToString()}");
            builder.WithColor(Color.DarkPurple);

            await ReplyAsync("", false, builder.Build());
        }

        [Command("mute")]
        [RequireUserPermission(GuildPermission.MuteMembers)]

        public async Task Mute(SocketGuildUser user = null)
        {

            if (user == null)
            {
                EmbedBuilder nouser = new EmbedBuilder();

                nouser.WithTitle("Please mention a user!");
                nouser.WithDescription("I don't know who to mute!");
                nouser.WithFooter($"{Context.Message.Author.ToString()}");
                nouser.WithCurrentTimestamp();
                nouser.WithColor(Color.DarkPurple);
                await ReplyAsync("", false, nouser.Build());
                return;
            }

            var muteRole = Context.Guild.Roles.SingleOrDefault(x => x.Name.Equals("Muted", StringComparison.OrdinalIgnoreCase) && !x.Permissions.SendMessages);
            if (muteRole == null)
            {
                EmbedBuilder nomuterole = new EmbedBuilder();

                nomuterole.WithTitle("No mute role found.");
                nomuterole.WithDescription("Please create a role named `Muted` without SendMessages permission.");
                nomuterole.WithCurrentTimestamp();
                nomuterole.WithColor(Color.DarkPurple);
                nomuterole.WithFooter($"{Context.Message.Author.ToString()}");

                await ReplyAsync("", false, nomuterole.Build());
                return;
            }
            if (user.Roles.Any(x => x.Name.Equals("Muted", StringComparison.OrdinalIgnoreCase)))
            {
                await user.RemoveRoleAsync(muteRole);

                EmbedBuilder unmuted = new EmbedBuilder();

                unmuted.WithTitle($"Unmuted {user.ToString()}");
                unmuted.WithDescription($"Successfully unmuted.");
                unmuted.WithCurrentTimestamp();
                unmuted.WithFooter($"{Context.Message.Author.ToString()}");
                unmuted.WithColor(Color.DarkBlue);

                await ReplyAsync("", false, unmuted.Build());

                return;
            }
            else
            {
                await user.AddRoleAsync(muteRole);

                EmbedBuilder muted = new EmbedBuilder();

                muted.WithTitle($"Muted {user.ToString()}");
                muted.WithDescription("Successfully muted.");
                muted.WithCurrentTimestamp();
                muted.WithColor(Color.DarkerGrey);
                muted.WithFooter($"{Context.Message.Author.ToString()}");

                await ReplyAsync("", false, muted.Build());
            }
        }



        [Command("Purge")]
        [Summary("Removes the specified number of messages.")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]

        public async Task Purge(int amount = 0)
        {

            if(amount == 0)
            {
                EmbedBuilder noargs = new EmbedBuilder();

                noargs.WithTitle("Please provide the number of messages to delete!");
                noargs.WithDescription("No message deletion amount provided.");
                noargs.WithCurrentTimestamp();
                noargs.WithFooter($"{Context.Message.Author.ToString()}");
                noargs.WithColor(Color.DarkPurple);

                await ReplyAsync("", false, noargs.Build());
                return;
            }

            // Check if the amount provided by the user is positive.
            if (amount <= 0)
            {

                EmbedBuilder belowzero = new EmbedBuilder();

                belowzero.WithTitle("The amount of messages to delete must be positive!");
                belowzero.WithDescription("Message deletion amount was a negative number.");
                belowzero.WithCurrentTimestamp();
                belowzero.WithFooter($"{Context.Message.Author.ToString()}");
                belowzero.WithColor(Color.DarkPurple);

                await ReplyAsync("", false, belowzero.Build());
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
                await Context.Message.DeleteAsync();
            }
        }


        [Command("ban")]
        [Summary("Ban's the specified user")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]

        public async Task Ban(SocketGuildUser user = null, [Remainder] string reason = null)
        {
            if (user == null)
            {
                EmbedBuilder nouser = new EmbedBuilder();

                nouser.WithTitle("Please mention a user!");
                nouser.WithDescription("I don't know who to ban!");
                nouser.WithFooter($"{Context.Message.Author.ToString()}");
                nouser.WithCurrentTimestamp();
                nouser.WithColor(Color.DarkPurple);
                await ReplyAsync("", false, nouser.Build());
                return;
            }

            if (reason == null)
            {
                EmbedBuilder noreason = new EmbedBuilder();

                noreason.WithTitle("Please provide a reason!");
                noreason.WithDescription("A ban reason is required.");
                noreason.WithFooter($"{Context.Message.Author.ToString()}");
                noreason.WithCurrentTimestamp();
                noreason.WithColor(Color.DarkPurple);
                await ReplyAsync("", false, noreason.Build());
                return;
            }

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"User {user.ToString()} was banned.");
            builder.WithDescription($"Banned for {reason}.");
            builder.WithFooter($"Banned by {Context.Message.Author.ToString()}");
            builder.WithCurrentTimestamp();

            builder.WithColor(Color.Red);
            await ReplyAsync("", false, builder.Build());

            EmbedBuilder builder2 = new EmbedBuilder();

            builder2.WithTitle($"{Context.Guild}");
            builder2.WithDescription($"You were banned for {reason}.");
            builder2.WithFooter($"Banned by {Context.Message.Author.ToString()}");
            builder2.WithCurrentTimestamp();

            await user.SendMessageAsync($"{user.Mention}", false, builder2.Build());
            await ReplyAsync($"{user.Mention}");
            await Context.Guild.AddBanAsync(user, 7, reason);
        }



        [Command("kick")]
        [Summary("Kick the specified user.")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser user = null, [Remainder] string reason = null)
        {
            if (user == null)
            {
                EmbedBuilder nouser = new EmbedBuilder();

                nouser.WithTitle("Please mention a user!");
                nouser.WithDescription("I don't know who to kick!");
                nouser.WithFooter($"{Context.Message.Author.ToString()}");
                nouser.WithCurrentTimestamp();
                nouser.WithColor(Color.DarkPurple);
                await ReplyAsync("", false, nouser.Build());
                return;
            }

            if (reason == null)
            {
                EmbedBuilder noreason = new EmbedBuilder();

                noreason.WithTitle("Please provide a reason!");
                noreason.WithDescription("A kick reason is required.");
                noreason.WithFooter($"{Context.Message.Author.ToString()}");
                noreason.WithCurrentTimestamp();
                noreason.WithColor(Color.DarkPurple);
                await ReplyAsync("", false, noreason.Build());
                return;
            }


            EmbedBuilder kicked = new EmbedBuilder();

            kicked.WithTitle($"User {user.ToString()} kicked.");
            kicked.WithDescription($"Kicked for {reason}.");
            kicked.WithFooter($"Kicked by {Context.Message.Author.ToString()}");
            kicked.WithCurrentTimestamp();
            kicked.WithColor(Color.DarkPurple);

            await ReplyAsync("", false, kicked.Build());

            EmbedBuilder kicked2 = new EmbedBuilder();

            kicked2.WithTitle($"{Context.Guild}");
            kicked2.WithDescription($"You were kicked for {reason}.");
            kicked2.WithFooter($"Kicked by {Context.Message.Author.ToString()}");
            kicked2.WithColor(Color.DarkPurple);

            await user.SendMessageAsync("", false, kicked2.Build());
            await user.KickAsync(reason);
        }
    }
}