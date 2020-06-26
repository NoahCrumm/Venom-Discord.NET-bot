using Discord;
using Newtonsoft.Json;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VenomBot.Modules
{
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    public class Admin : ModuleBase<SocketCommandContext>
    {
        static string ModLogsPath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Modlogs.json";
        static async Task AddModlogs(ulong userID, Action action, ulong ModeratorID, string reason, string username)
        {
            if (currentLogs.Users.Any(x => x.userId == userID))
            {
                currentLogs.Users[currentLogs.Users.FindIndex(x => x.userId == userID)].Logs.Add(new UserModLogs()
                {
                    Action = action,
                    ModeratorID = ModeratorID,
                    Reason = reason,
                    Date = DateTime.UtcNow.ToString("r")
                });
            }
            else
            {
                currentLogs.Users.Add(new User()
                {
                    Logs = new List<UserModLogs>()
                    {
                        { new UserModLogs(){
                            Action = action,
                            ModeratorID = ModeratorID,
                            Reason = reason,
                            Date = DateTime.UtcNow.ToString("r")
                        } }
                    },
                    userId = userID,
                    username = username
                });
            }
            SaveModLogs();
        }

        static ModlogsJson LoadModLogs()
        {
            try
            {
                var d = JsonConvert.DeserializeObject<ModlogsJson>(File.ReadAllText(ModLogsPath));
                if (d == null) { throw new Exception(); }
                return d;
            }
            catch //(Exception ex)
            {
                return new ModlogsJson() { Users = new List<User>() };
            }


        }

        public static ModlogsJson currentLogs { get; set; } = LoadModLogs();
        static public void SaveModLogs()
        {
            string json = JsonConvert.SerializeObject(currentLogs);
            File.WriteAllText(ModLogsPath, json);
        }

        public class ModlogsJson
        {
            public List<User> Users { get; set; }
        }
        public class User
        {
            public List<UserModLogs> Logs { get; set; }
            public ulong userId { get; set; }
            public string username { get; set; }
        }
        public class UserModLogs
        {
            public string Reason { get; set; }
            public Action Action { get; set; }
            public ulong ModeratorID { get; set; }
            public string Date { get; set; }
        }

        public enum Action
        {
            Warned,
            Kicked,
            Banned,
            Muted,
            voiceban
        }

        [Command("clearlog")]
        public async Task clearwarn(string user1, int number = 999)
        {
            var user = Context.User as SocketGuildUser;
            var roleStaff = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Staff");
            var mentions = Context.Message.MentionedUsers;

            if (!user.GuildPermissions.ManageMessages)
            {
                await Context.Channel.SendMessageAsync("", false, new Discord.EmbedBuilder()
                {
                    Title = "You do not have permission to execute this command",
                    Description = "You do not have the valid permission to execute this command",
                    Color = Color.Red
                }.Build());
                return;
            }

            Regex r = new Regex("(\\d{18}|\\d{17})");
            if (!r.IsMatch(user1))
            {
                await Context.Channel.SendMessageAsync("", false, new Discord.EmbedBuilder()
                {
                    Title = "Invalid ID",
                    Description = "The ID you provided is invalid!",
                    Color = Color.Red
                }.Build());
                return;
            }
            ulong id;
            try
            {
                id = Convert.ToUInt64(r.Match(user1).Groups[1].Value);
            }
            catch
            {
                await Context.Channel.SendMessageAsync("", false, new Discord.EmbedBuilder()
                {
                    Title = "Invalid ID",
                    Description = "The ID you provided is invalid!",
                    Color = Color.Red
                }.Build());
                return;
            }

            var num = number.ToString();

            if (num == "999")
            {
                if (currentLogs.Users.Any(x => x.userId == id))
                {
                    var usrlogs = currentLogs.Users[currentLogs.Users.FindIndex(x => x.userId == id)];
                    string usrnm = Context.Guild.GetUser(usrlogs.userId) == null ? usrlogs.username : Context.Guild.GetUser(usrlogs.userId).ToString();
                    EmbedBuilder b = new EmbedBuilder()
                    {
                        Title = $"Modlogs for **{usrnm}**",
                        Color = Color.DarkMagenta,
                        Description = $"Modlogs for {usrlogs.username},\nTo remove a log type `!clearlog <user> <log number>`\n",
                        Fields = new List<EmbedFieldBuilder>()
                    };
                    for (int i = 0; i != usrlogs.Logs.Count; i++)
                    {
                        var log = usrlogs.Logs[i];
                        b.Fields.Add(new EmbedFieldBuilder()
                        {
                            IsInline = false,
                            Name = (i + 1).ToString(),
                            Value =
                            $"**{log.Action}**\n" +
                            $"Reason: {log.Reason}\n" +
                            $"Moderator: <@{log.ModeratorID}> ({log.ModeratorID.ToString()}\n" +
                            $"Date: {log.Date}"
                        });
                    }
                    await Context.Channel.SendMessageAsync("", false, b.Build());
                    return;
                }
                else
                {
                    EmbedBuilder b = new EmbedBuilder()
                    {
                        Title = "User has no logs!",
                        Description = $"The user <@{id}> has no logs!",
                        Color = Color.Red,
                    };
                    await Context.Channel.SendMessageAsync("", false, b.Build());
                    return;
                }
            }

            if (currentLogs.Users.Any(x => x.userId == id))
            {
                var usrlogs = currentLogs.Users[currentLogs.Users.FindIndex(x => x.userId == id)];
                usrlogs.Logs.RemoveAt(number - 1);
                string usrnm = Context.Guild.GetUser(usrlogs.userId) == null ? usrlogs.username : Context.Guild.GetUser(usrlogs.userId).ToString();
                EmbedBuilder b = new EmbedBuilder()
                {
                    Title = $"Successfully cleared a log for **{usrnm}**",
                    Color = Color.DarkMagenta,
                    Description = $"Modlogs for {usrlogs.username},\nTo remove a log type `!clearlog <user> <log number>`\n",
                    Fields = new List<EmbedFieldBuilder>()
                };
                for (int i = 0; i != usrlogs.Logs.Count; i++)
                {
                    var log = usrlogs.Logs[i];
                    b.Fields.Add(new EmbedFieldBuilder()
                    {
                        IsInline = false,
                        Name = (i + 1).ToString(),
                        Value =
                        $"**{log.Action}**\n" +
                        $"Reason: {log.Reason}\n" +
                        $"Moderator: <@{log.ModeratorID}> ({log.ModeratorID.ToString()}\n" +
                        $"Date: {log.Date}"
                    });
                }
                await Context.Channel.SendMessageAsync("", false, b.Build());
            }
            else
            {
                EmbedBuilder b = new EmbedBuilder()
                {
                    Title = "User has no logs!",
                    Description = $"The user <@{id}> has no logs!",
                    Color = Color.Red,
                };
                await Context.Channel.SendMessageAsync("", false, b.Build());
            }

        }

        [Command("modlogs")]
        public async Task Modlogs(string mention)
        {
            var user = Context.User as SocketGuildUser;
            var roleStaff = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Staff");
            var mentions = Context.Message.MentionedUsers;
            if (!user.GuildPermissions.ManageMessages)
            {
                await Context.Channel.SendMessageAsync("", false, new Discord.EmbedBuilder()
                {
                    Title = "You do not have permission to execute this command",
                    Description = "You do not have the valid permission to execute this command",
                    Color = Color.Red
                }.Build());
                return;
            }

            if (mentions.Count == 0)
            {
                await Context.Channel.SendMessageAsync("test", false);
                return;
            }

            var user1 = mentions.First();

            if (currentLogs.Users.Any(x => x.userId == user1.Id))
            {
                var userAcount = currentLogs.Users[currentLogs.Users.FindIndex(x => x.userId == user1.Id)];
                var logs = userAcount.Logs;
                string usrnm = Context.Guild.GetUser(userAcount.userId) == null ? userAcount.username : Context.Guild.GetUser(userAcount.userId).ToString();
                EmbedBuilder b = new EmbedBuilder()
                {
                    Title = $"Modlogs for **{usrnm}** ({user1.Id})",
                    Description = $"To remove a log type `!clearlog <user> <log number>`",
                    Color = Color.Green,
                    Fields = new List<EmbedFieldBuilder>()
                };
                foreach (var log in logs)
                {
                    b.Fields.Add(new EmbedFieldBuilder()
                    {
                        IsInline = false,
                        Name = Enum.GetName(typeof(Action), log.Action),
                        Value = $"Reason: {log.Reason}\nModerator: <@{log.ModeratorID}>\nDate: {log.Date}"
                    });
                }
                if (logs.Count == 0)
                {
                    b.Description = "This user has not logs!";
                }
                await Context.Channel.SendMessageAsync("", false, b.Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync("", false, new Discord.EmbedBuilder()
                {
                    Title = $"Modlogs for ({user1.Id})",
                    Description = "This user has no logs! :D",
                    Color = Color.Green
                }.Build());
                return;
            }
        }


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

        public async Task Mute(SocketGuildUser user = null, [Remainder] string reason = null)
        {
            try
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
                    await AddModlogs(user.Id, Action.Muted, Context.Message.Author.Id, reason, user.Username);
                    return;
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);

                EmbedBuilder builder = new EmbedBuilder();

                builder.WithTitle("Mute role must not be above the bot role.");
                builder.WithCurrentTimestamp();
                builder.WithColor(Color.DarkPurple);
                builder.WithFooter($"{Context.Message.Author.ToString()}");

                await ReplyAsync($"{Context.Message.Author.Mention}", false, builder.Build());
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
