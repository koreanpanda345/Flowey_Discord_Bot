using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Flowey.Html;

namespace Flowey.Bot.Core.Commands
{
    class DevCommands : ModuleBase<SocketCommandContext>
    {
        [Command("userjoin")]
        public async Task UserJoinCommand()
        {
            var html = new Welcome(Context.User.Username, Context.User.GetAvatarUrl());
            var channel = Context.Client.GetChannel(671139806549508117) as SocketTextChannel;
            await channel.SendFileAsync(new MemoryStream(await html.CreateImage()), "welcome.jpg", "");
        }
    }
}
