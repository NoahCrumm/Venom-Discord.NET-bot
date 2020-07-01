namespace VenomBot.Modules
{
    using System;
    using System.Threading.Tasks;
    using Discord.Commands;
    using Discord;
    using Lavalink4NET;
    using Lavalink4NET.DiscordNet;
    using Lavalink4NET.Player;
    using Lavalink4NET.Rest;

    /// <summary>
    ///     Presents some of the main features of the Lavalink4NET-Library.
    /// </summary>
    [Name("Music")]
    [RequireContext(ContextType.Guild)]
    public sealed class MusicModule : ModuleBase<SocketCommandContext>
    {
        private readonly IAudioService _audioService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MusicModule"/> class.
        /// </summary>
        /// <param name="audioService">the audio service</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="audioService"/> is <see langword="null"/>.
        /// </exception>
        public MusicModule(IAudioService audioService)
            => _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));

        /// <summary>
        ///     Disconnects from the current voice channel connected to asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        [Command("disconnect", RunMode = RunMode.Async)]
        [Alias("leave")]
        public async Task Disconnect()
        {
            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;
            }

            // when using StopAsync(true) the player also disconnects and clears the track queue.
            // DisconnectAsync only disconnects from the channel.

            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle("Disconnected.")
                .WithCurrentTimestamp()
                .WithColor(Color.DarkPurple);

            await player.StopAsync(true);
            await ReplyAsync("", false, builder.Build());
        }

        /// <summary>
        ///     Plays music from YouTube asynchronously.
        /// </summary>
        /// <param name="query">the search query</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        [Command("play", RunMode = RunMode.Async)]
        public async Task Play([Remainder] string query)
        {
            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;
            }

            var track = await _audioService.GetTrackAsync(query, SearchMode.YouTube);

            if (track == null)
            {
                EmbedBuilder builder = new EmbedBuilder()
                    .WithTitle("Venom Music Module")
                    .WithDescription($"No results found for {query}.")
                    .WithColor(new Color(75, 22, 189))
                    .WithCurrentTimestamp();
                await ReplyAsync("", false, builder.Build());
                return;
            }

            var position = await player.PlayAsync(track, enqueue: true);

            if (position == 0)
            {
                EmbedBuilder playing = new EmbedBuilder()
                    .WithTitle("Venom Music Module")
                    .WithDescription($"Now playing **{track.Title}**.")
                     .WithColor(new Color(75, 22, 189))
                    .WithCurrentTimestamp();
                await ReplyAsync("", false, playing.Build());
                await player.SetVolumeAsync(50 / 100f);
            }
            else
            {
                await ReplyAsync("Queue currently not functional : (");
            }
        }

        /// <summary>
        ///     Shows the track position asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        [Command("position", RunMode = RunMode.Async)]
        public async Task Position()
        {
            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;
            }

            if (player.CurrentTrack == null)
            {
                await ReplyAsync("Nothing playing!");
                return;
            }

            await ReplyAsync($"Position: {player.TrackPosition} / {player.CurrentTrack.Duration}.");
        }

        /// <summary>
        ///     Stops the current track asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        [Command("stop", RunMode = RunMode.Async)]
        public async Task Stop()
        {
            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;
            }

            if (player.CurrentTrack == null)
            {
                await ReplyAsync("Nothing playing!");
                return;
            }

            await player.StopAsync();
            await ReplyAsync("Stopped playing.");
        }

        /// <summary>
        ///     Updates the player volume asynchronously.
        /// </summary>
        /// <param name="volume">the volume (1 - 1000)</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        [Command("volume", RunMode = RunMode.Async)]
        public async Task Volume(int volume = 100)
        {
            if (volume > 1000 || volume < 0)
            {
                EmbedBuilder volumehigh = new EmbedBuilder()
                    .WithTitle("Venom Music Module")
                    .WithDescription("Volume out of range: 0 % -1000 % !")
                    .WithCurrentTimestamp()
                    .WithColor(new Color(75, 22, 189));
                await ReplyAsync("", false, volumehigh.Build());
                return;
            }

            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;
            }

            EmbedBuilder volumeset = new EmbedBuilder()
                .WithTitle("Venom Music Module")
                .WithDescription($"Volume updated {volume}%")
                .WithColor(new Color(75, 22, 189))
                .WithCurrentTimestamp();

            await player.SetVolumeAsync(volume / 200f);
            await ReplyAsync($"", false, volumeset.Build());
        }

        /// <summary>
        ///     Gets the guild player asynchronously.
        /// </summary>
        /// <param name="connectToVoiceChannel">
        ///     a value indicating whether to connect to a voice channel
        /// </param>
        /// <returns>
        ///     a task that represents the asynchronous operation. The task result is the lavalink player.
        /// </returns>
        private async Task<VoteLavalinkPlayer> GetPlayerAsync(bool connectToVoiceChannel = true)
        {
            var player = _audioService.GetPlayer<VoteLavalinkPlayer>(Context.Guild.Id);

            if (player != null
                && player.State != PlayerState.NotConnected
                && player.State != PlayerState.Destroyed)
            {
                return player;
            }

            var user = Context.Guild.GetUser(Context.User.Id);

            if (!user.VoiceState.HasValue)
            {
                await ReplyAsync("You must be in a voice channel!");
                return null;
            }

            if (!connectToVoiceChannel)
            {
                await ReplyAsync("The bot is not in a voice channel!");
                return null;
            }

            return await _audioService.JoinAsync<VoteLavalinkPlayer>(user.VoiceChannel);
        }
    }
}