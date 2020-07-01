using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Diagnostics;

namespace VenomBot.Modules
{

    [Name("Owner")]
    [RequireOwner]

    public class OwnerOnly : ModuleBase<SocketCommandContext>
    {

        [Command("setgame")]
        [Summary("Set's the bots game")]
        [Remarks("setgame {game}")]

        public async Task SetGameAsync([Remainder] string game)
        {
            await ReplyAsync($"Set game to {game}");
            await Context.Client.SetGameAsync(game);
        }

        [Command("restart")]
        [Summary("Restarts the bot")]

        public async Task Restart()
        {
            Process process = null;
            try
            {
                process = Process.GetCurrentProcess();
                Process.Start("VenomBot");

                EmbedBuilder builder = new EmbedBuilder()
                    .WithTitle("Successfully restarted.")
                    .WithColor(Color.DarkPurple)
                    .WithCurrentTimestamp();
                await ReplyAsync("", false, builder.Build());

                Environment.Exit(0);
            }
            catch
            {
                await ReplyAsync("Restart failed!");
            }
        }

    }
}
