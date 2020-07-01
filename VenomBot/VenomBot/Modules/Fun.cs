using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using System;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using VenomBot.Services;
using System.Text.RegularExpressions;
using Discord.WebSocket;

namespace VenomBot.Modules
{
    public class Fun : InteractiveBase<SocketCommandContext>
    {

        static int rot = 0;

        [Command("roll")]
		[Summary("Roles between 0 and 50 or between two custom numbers")]
		public async Task Roll(int min = 0, int max = 50)
		{
			Random r = new Random();
			int random = r.Next(min, max);
			await Context.Channel.SendMessageAsync("The number was: " + random);
		}

        [Command("lovetest")]
        [Summary("Measures the amount of love between two people or objects.")]

        public async Task RNG(string love1, [Remainder] string love)
        {

            var rand = new Random();

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Love Test");
            builder.WithDescription($"How much does {love1.ToString()} love {love.ToString()}");
            builder.AddField(rand.Next(101).ToString(), "%");

            await ReplyAsync("", false, builder.Build());
        }

        [Command("CursedImage")]
        [Summary("Grabs a cursed image from Reddit.")]
        [RequireNsfw]

        public async Task imagesearch()
        {
            HttpClient c = new HttpClient();
            var req = await c.GetAsync("https://www.reddit.com/r/cursedimages.json");
            string resp = await req.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<RedditHandler>(resp);
            Regex r = new Regex(@"https:\/\/i.redd.it\/(.*?)\.");
            var childs = data.Data.Children.Where(x => r.IsMatch(x.Data.Url.ToString()));
            Random rnd = new Random();
            int count = childs.Count();
            if (rot >= count - 1)
                rot = 0;
            var post = childs.ToArray()[rot];
            rot++;

            EmbedBuilder b = new EmbedBuilder()
            {
                Title = "Cursed",
                ImageUrl = post.Data.Url.ToString(),
                Footer = new EmbedFooterBuilder()
                {
                    Text = "u/" + post.Data.Author
                }
            };

            b.WithCurrentTimestamp();
            b.WithColor(Color.DarkPurple);

            await Context.Channel.SendMessageAsync("", false, b.Build());
        }

        [Command("8ball")]
        [Summary("Ask a question and it shall be answered.")]
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
        [Summary("Displays a picture of a random politcian.")]

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


        [Command("bork")]
        [Summary("Show's a picture of a dog barking.")]
        public async Task BorkCommand([Remainder] IUser user = null)
        {

            if (user == null)
            {
                EmbedBuilder builder2 = new EmbedBuilder();

                builder2.WithTitle($"{Context.Message.Author.ToString()} borked.");
                builder2.WithImageUrl("https://moderndogmagazine.com/sites/default/files/images/articles/top_images/barkingdogs.JPG");
                builder2.WithCurrentTimestamp();

                builder2.WithColor(Color.DarkBlue);
                await ReplyAsync("", false, builder2.Build());
                return;
            }

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"{Context.Message.Author.ToString()} borked at {user.ToString()}.");
            builder.WithImageUrl("https://moderndogmagazine.com/sites/default/files/images/articles/top_images/barkingdogs.JPG");
            builder.WithCurrentTimestamp();

            builder.WithColor(Color.DarkBlue);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("rice")]
        [Summary("Displays a random picture of rice.")]
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
