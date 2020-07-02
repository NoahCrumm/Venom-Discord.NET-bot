using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using VenomBot.Services;
using Discord.WebSocket;

namespace VenomBot.Modules
{

    [Name("🤖 Commands")]

    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    public class Commands : InteractiveBase<SocketCommandContext>
    {

        private static readonly Color Purplism = new Color(38, 0, 99);


        static int rot = 0;

        [Command("server")]
        [Summary("Gets details about the server you are in")]
        public async Task ServerGuild()
        {
            SocketGuildUser guildUser = (SocketGuildUser)Context.User;

            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("Server Details");
            embed.WithDescription("**__Server__**" +
                                  $"\n**Server Name:** {guildUser.Guild}" +
                                  $"\n**Server Id:** {guildUser.Guild.Id}" +
                                  $"\n**Server Member Count:** {guildUser.Guild.MemberCount}" +
                                  "\n\n**__Server Owner__**" +
                                  $"\n**Owner Name: **{guildUser.Guild.Owner.Username}" +
                                  $"\n**Owner Id: ** {guildUser.Guild.OwnerId}");
            embed.WithThumbnailUrl(guildUser.Guild.IconUrl);
            embed.WithColor(Color.DarkPurple);

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

            [Command("Uptime")]
            [Summary("Tells how long the bot has been online.")]
            [Remarks("uptime")]
            public async Task UptimeAsync()
            {
                await ReplyAsync($"I've been online for **{Process.GetCurrentProcess().PrivilegedProcessorTime}**!");
            }

        [Command("ping")]
        [Summary("Checks the ping of the bot.")]

        public async Task Ping()
        {

            EmbedBuilder ping = new EmbedBuilder();

            ping.WithTitle("Pong :ping_pong:");
            ping.WithDescription($"{Context.Client.Latency}ms!");
            ping.WithColor(Purplism);
            ping.WithCurrentTimestamp();
            await ReplyAsync("", false, ping.Build());
        }

        [Command("guilds")]
        [Summary("Lists all of the guilds the bot is in.")]
        [RequireOwner]

        public async Task Network()
        {

            EmbedBuilder builder = new EmbedBuilder();

            var table = new Table("Name", "Owner", "Users");
            foreach (var guild in Context.Client.Guilds)
            {
                table.AddRow(guild.Name, guild.Owner.Username, guild.Users.Count().ToString());

                var channel = guild.DefaultChannel ?? guild.TextChannels.FirstOrDefault();
                if (channel != null)
                {

                }
            }

            builder.WithTitle("Guilds");
            builder.WithDescription(table.ToString());

            await ReplyAsync("", false, builder.Build());
        }

        [Command("avatar")]
        [Alias("av", "ava", "pvp")]
        [Summary("Gets the avatar of a user.")]

        public async Task GetAvatar(IUser user)
        {
            var avatarUrl = user.GetAvatarUrl();

            var embed = new EmbedBuilder()
                .WithTitle($"{user.Username}'s avatar")
                .WithImageUrl(avatarUrl)
                .WithCurrentTimestamp()
                .WithColor(Color.DarkPurple)
                .WithFooter($"{Context.User.Username}")
                .WithCurrentTimestamp()
                .Build();

            await ReplyAsync(embed: embed);
            await Context.Message.DeleteAsync();
        }
    }
}