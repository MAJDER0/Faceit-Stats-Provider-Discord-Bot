using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using System.Text;
using Discord_Bot_Faceit_Stats_Provider.Commands;
using System.IO;
using Discord_Bot_Faceit_Stats_Provider;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.EventArgs;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Discord_Bot_Faceit_Stats_Provider
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync(IServiceProvider serviceProvider)
        {

            var json = string.Empty;
            using (var fs = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "..", "..", "config.json")))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            var configJson = JsonConvert.DeserializeObject<ConfigJSON>(json);


            var config = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };


            Client = new DiscordClient(config);

            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            var commands = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                Services = serviceProvider,
                StringPrefixes = new string[] { configJson.Prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            });

            commands.RegisterCommands(Assembly.GetExecutingAssembly());

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
