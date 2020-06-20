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
            var sb = new StringBuilder();

            var embed = new EmbedBuilder();

            var replies = new List<string>();

            // Possible Replies

            replies.Add("1");
            replies.Add("2");
            replies.Add("3");
            replies.Add("4");
            replies.Add("5");
            replies.Add("6");

            embed.WithColor(Color.DarkPurple);
            embed.Title = "Rice";

            var answer = replies[new Random().Next(replies.Count)];

            sb.AppendLine($"{answer}");

            switch (answer)
            {
                case "1":
                    {
                        embed.WithImageUrl("https://content.jwplatform.com/thumbs/8Rt7lo19-720.jpg");
                        break;
                    }
                case "2":
                    {
                        embed.WithImageUrl("https://www.pressurecookrecipes.com/wp-content/uploads/2018/06/instant-pot-rice.jpg");
                        break;
                    }
                case "3":
                    {
                        embed.WithImageUrl("https://hips.hearstapps.com/vidthumb/images/delish-spanish-rice-still003-1544641098.jpg");
                        break;
                    }
                case "4":
                    {
                        embed.WithImageUrl("https://www.favfamilyrecipes.com/wp-content/uploads/2018/10/Restaurant-Style-Mexican-Rice-500x500.jpg");
                        break;
                    }
                case "5":
                    {
                        embed.WithImageUrl("https://kristineskitchenblog.com/wp-content/uploads/2019/11/instant-pot-white-rice-1200-1156.jpg");
                        break;
                    }
                case "6":
                    {
                        embed.WithImageUrl("https://www.saveur.com/resizer/pwgtC3iPvl0rBX8QL5QQPfmMxNk=/760x570/arc-anglerfish-arc2-prod-bonnier.s3.amazonaws.com/public/YHPWXPU4TJ6I7N566AJQ2HXNBE.jpg");
                        break;
                    }
            }

            embed.WithFooter($"{Context.Message.Author.ToString()}");

            await ReplyAsync(null, false, embed.Build());
        }
    }
}