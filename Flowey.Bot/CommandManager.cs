using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
namespace Flowey.Bot
{
    public class CommandManager
    {
        private CommandService _Commands;
        private readonly IServiceProvider _Service;
        public CommandManager(IServiceProvider Service)
        {
            _Service = Service;
            _Commands = _Service.GetService<CommandService>();
        }

        public async Task InitAsync()
        {
            await _Commands.AddModulesAsync(Assembly.GetEntryAssembly(), _Service);
            foreach(var cmd in _Commands.Commands)
            {
                Console.WriteLine($"{cmd.Name} was loaded.");
            }
        }
    }
}
