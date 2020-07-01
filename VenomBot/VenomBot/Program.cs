using System;
using System.IO;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Discord.Addons.Interactive;
using Microsoft.Extensions.DependencyInjection;
using VenomBot.Services;
using Lavalink4NET;
using Lavalink4NET.Logging;
using Lavalink4NET.MemoryCache;
using Lavalink4NET.DiscordNet;

namespace VenomBot
{
    class Program
    {
        // setup our fields we assign later
        private readonly IConfiguration _config;
        private DiscordSocketClient _client;

        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            // create the configuration
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");
                

            // build the configuration and assign to _config          
            _config = _builder.Build();
        }

        public async Task MainAsync()
        {
            // call ConfigureServices to create the ServiceCollection/Provider for passing around the services
            using (var services = ConfigureServices())
            {
                // get the client and assign to client 
                // you get the services via GetRequiredService<T>
                var client = services.GetRequiredService<DiscordSocketClient>();
                var audio = services.GetRequiredService<IAudioService>();
                var logger = services.GetRequiredService<ILogger>() as EventLogger;
                _client = client;

                // setup logging and the ready event
                client.Log += LogAsync;
                client.Ready += ReadyAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                // this is where we get the Token value from the configuration file, and start the bot
                await client.LoginAsync(TokenType.Bot, _config["Token"]);
                await client.StartAsync();
                await audio.InitializeAsync();

                //Set the game name and 
                string gameName = "Version 1.1.2 <3";
                await _client.SetGameAsync(gameName);
                await _client.SetStatusAsync(UserStatus.DoNotDisturb);

                // we get the CommandHandler class here and call the InitializeAsync method to start things up for the CommandHandler service
                await services.GetRequiredService<CommandHandler>().InitializeAsync();

                await Task.Delay(-1);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"Connected as -> [{_client.CurrentUser}] :)");
            Console.WriteLine("[" + DateTime.Now.TimeOfDay + "] - " + "Welcome, " + Environment.UserName + ".");
            return Task.CompletedTask;
        }




        // this method handles the ServiceCollection creation/configuration, and builds out the service provider we can call on later
        private ServiceProvider ConfigureServices()
        {
            // this returns a ServiceProvider that is used later to call for those services
            // we can add types we have access to here, hence adding the new using statement:
            // using csharpi.Services;
            // the config we build is also added, which comes in handy for setting the command prefix!
            return new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton<IAudioService, LavalinkNode>()
                .AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>()
                .AddSingleton<ILogger, EventLogger>()
                .AddSingleton(new LavalinkNodeOptions
                {
                    RestUri = "http://10.0.0.106:2333/",
                    WebSocketUri = "ws://10.0.0.106:2333/",
                    Password = "youshallnotpass"
                })
                .AddSingleton<InteractiveService>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }
    }
}
