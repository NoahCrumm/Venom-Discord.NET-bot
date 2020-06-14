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
    public class Commands : ModuleBase
    {
        [Command("help")]
        public async Task HelpCommand()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Help");
            builder.AddField("Ping", "Use this command to test if the bot is online!");    // true - for inline
            builder.AddField("Venom", "Use this command to praise the king!");
            builder.AddField("Rice", "This command displays a picture of rice.");
            builder.AddField("uwu daddy", "This command is currently broken!");
            builder.WithThumbnailUrl("https://www.dropbox.com/s/e1aau7lsfv3bvuo/Circle-Transparent.png?dl=1");

            builder.WithColor(Color.Red);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("8ball")]
        [Alias("ask")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task AskEightBall([Remainder]string args = null)
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
            embed.WithColor(new Color(0, 255,0));
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
                        embed.WithColor(new Color(255,255,0));
                        break;
                    }
                    case "hazy... Please ask again later.":
                    {
                        embed.WithColor(new Color(255,0,255));
                        break;
                    }
                }
            }

            embed.Description = sb.ToString();

            // this will reply with the embed
            await ReplyAsync(null, false, embed.Build());
        }
    }
}