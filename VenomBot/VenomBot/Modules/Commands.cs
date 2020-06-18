using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using VenomBot.Services;
using Microsoft.Extensions.Configuration;
using System.Dynamic;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace VenomBot.Modules
{
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]

        public async Task Ping()
        {
            EmbedBuilder ping = new EmbedBuilder();

            ping.WithTitle("Pong :ping_pong:");
            ping.WithDescription($"{Context.Client.Latency}ms!");
            ping.WithColor(Color.DarkBlue);
            ping.WithCurrentTimestamp();
            await ReplyAsync("", false, ping.Build());
        }

        [Command("help")]
        public async Task HelpCommand()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Help");
            builder.AddField("Ping", "Use this command to test if the bot is online!");    // true - for inline
            builder.AddField($"Rice", "ricey dicey.");
            builder.AddField("Bork", "Bork at your friends : O");
            builder.WithThumbnailUrl("https://www.dropbox.com/s/e1aau7lsfv3bvuo/Circle-Transparent.png?dl=1");

            builder.WithColor(Color.DarkPurple);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("8ball")]
        [Alias("ask")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task AskEightBall([Remainder] string args = null)
        {

            var sb = new StringBuilder();

            var embed = new EmbedBuilder();

            var replies = new List<string>();

            // Possible Replies
            replies.Add("yes");
            replies.Add("no");
            replies.Add("maybe");
            replies.Add("hazzzzy....");

            // Color and Title of the embed
            embed.WithColor(new Color(0, 255, 0));
            embed.Title = "Welcome to the 8-ball!";

            sb.AppendLine($"{Context.User.Username},");
            sb.AppendLine();

            // let's make sure the supplied question isn't null 
            if (args == null)
            {
                // if no question is asked (args are null), reply with the below text
                sb.AppendLine("Sorry, can't answer a question you didn't ask!");
            }
            else
            {
                // get a random number to index our list with arrays
                var answer = replies[new Random().Next(replies.Count - 1)];

                // Build the reply thingy
                sb.AppendLine($"You asked: [**{args}**]...");
                sb.AppendLine();
                sb.AppendLine($"...your answer is [**{answer}**]");

                // bonus - let's switch out the reply and change the color based on it
                switch (answer)
                {
                    case "yes":
                        {
                            embed.WithColor(new Color(0, 255, 0));
                            break;
                        }
                    case "no":
                        {
                            embed.WithColor(new Color(255, 0, 0));
                            break;
                        }
                    case "maybe":
                        {
                            embed.WithColor(new Color(255, 255, 0));
                            break;
                        }
                    case "hazy... Please ask again later.":
                        {
                            embed.WithColor(new Color(255, 0, 255));
                            break;
                        }
                }
            }

            embed.Description = sb.ToString();

            // this will reply with the embed
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("politician")]

        public async Task politician([Remainder] string ignore = null)
        {

            var sb = new StringBuilder();

            var embed = new EmbedBuilder();

            var replies = new List<string>();

            // Possible Replies

            replies.Add("Abraham Lincoln");
            replies.Add("Donald Trump");
            replies.Add("George W. Bush");
            replies.Add("Joe Biden");
            replies.Add("Mike Pence");
            replies.Add("John F. Kennedy");

            embed.WithColor(Color.DarkRed);
            embed.Title = "Your politician is,";

            var answer = replies[new Random().Next(replies.Count)];

            sb.AppendLine($"{answer}");

            switch (answer)
            {
                case "Abraham Lincoln":
                    {
                        embed.WithImageUrl("https://www.biography.com/.image/t_share/MTU5MDUzMTE0Mzk2MTI0OTUy/abraham-lincoln-1809---18652c-sixteenth-president-of-the-united-states-of-america-photo-by-stock-montagestock-montagegetty-images_promo.jpg");
                        break;
                    }
                case "Donald Trump":
                    {
                        embed.WithImageUrl("https://thenypost.files.wordpress.com/2020/06/1219639689.jpg?quality=80&strip=all&w=618&h=410&crop=1");
                        break;
                    }
                case "George W. Bush":
                    {
                        embed.WithImageUrl("https://cdn.britannica.com/78/73178-050-00309E51/George-W-Bush.jpg");
                        break;
                    }
                case "Joe Biden":
                    {
                        embed.WithImageUrl("https://thehill.com/sites/default/files/styles/article_full/public/bidenjoe_030820getty_progressives.jpg?itok=I6cvXjtz");
                        break;
                    }
                case "Mike Pence":
                    {
                        embed.WithImageUrl("https://ewscripps.brightspotcdn.com/dims4/default/2e55ffd/2147483647/strip/true/crop/640x360+0+60/resize/1280x720!/quality/90/?url=https%3A%2F%2Fsharing.wxyz.com%2Fsharescnn%2Fphoto%2F2017%2F03%2F11%2F1489263934_56716217_ver1.0_640_480.jpg");
                        break;
                    }
                case "John F. Kennedy":
                    {
                        embed.WithImageUrl("https://www.history.com/.image/ar_1:1%2Cc_fill%2Ccs_srgb%2Cfl_progressive%2Cq_auto:good%2Cw_1200/MTU3ODc4Njc0MzgxMTU0MDE1/gettyimages-523204139-2.jpg");
                        break;
                    }
            }

            embed.Description = sb.ToString();
            embed.WithFooter($"{Context.Message.Author.ToString()}");

            await ReplyAsync(null, false, embed.Build());
        }

    }
}