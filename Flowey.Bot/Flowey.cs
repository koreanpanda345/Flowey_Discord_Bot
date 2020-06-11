using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using Microsoft.Extensions.DependencyInjection;
namespace Flowey.Bot
{
    public class Flowey
    {
        private DiscordSocketClient Client;
        private CommandService Commands;
        private IServiceProvider Service;
        public Flowey()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
            });
            Commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async
            });
            Service = BuilderServiceProvider();
        }

        public async Task MainAsync()
        {
            CommandManager cmd = new CommandManager(Service);
            await cmd.InitAsync();
            EventHandler events = new EventHandler(Service);
            await events.InitAsync();
            if (Config.Bot.Token == "" || Config.Bot.Token == null) return;
            await Client.LoginAsync(TokenType.Bot, Config.Bot.Token);
            await Client.StartAsync();
            await Task.Delay(-1);
        }

        private ServiceProvider BuilderServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .AddSingleton(new InteractiveService(Client))
                .BuildServiceProvider();
        }
    }
}
