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

namespace VenomBot.Modules
{
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    public class EasterEggs : ModuleBase
    {
        [Command("bork")]
        public async Task BorkCommand([Remainder] IUser user)
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithImageUrl("https://moderndogmagazine.com/sites/default/files/images/articles/top_images/barkingdogs.JPG");
            builder.WithCurrentTimestamp();

            builder.WithColor(Color.DarkBlue);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("authoritarian")]

        public async Task Authoritarian()
        {
            await ReplyAsync("lol");
        }

        [Command("rice")]
        public async Task RiceCommand()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Rice");
            builder.WithImageUrl("https://source.unsplash.com/random/?rice");
            builder.WithCurrentTimestamp();

            builder.WithColor(Color.Red);
            await ReplyAsync("", false, builder.Build());
        }
    }
}