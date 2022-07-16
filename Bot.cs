using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using MeeoBotDSharpPlus.Commands;
using Newtonsoft.Json;
using MingweiSamuel.Camille;
using DSharpPlus.Interactivity.Extensions;

namespace MeeoBotDSharpPlus
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("Config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                Intents = DiscordIntents.All,
                MessageCacheSize = 100,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
            };

            Client = new DiscordClient(config);

            //trying out stuff
            ReactionCounter rc = new ReactionCounter();
            Client.MessageReactionAdded += rc.ReactionsAddHandler;
            Client.MessageReactionRemoved += rc.ReactionsRemoveHandler;
            Client.MessageCreated += rc.MessagesAddHandler;


            Client.UseInteractivity(new InteractivityConfiguration { });

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableMentionPrefix = true,
                EnableDms = false
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<Test>();
            Commands.RegisterCommands<RiotCommands>();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }
    }
}
