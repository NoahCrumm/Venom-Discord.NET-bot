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
    public class Rice : ModuleBase
    {
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