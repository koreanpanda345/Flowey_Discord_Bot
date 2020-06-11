using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Addons.Interactive;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Linq;
using Flowey.Airtable;
using Flowey.Airtable.Objects;
using Flowey.Html;
using System.IO;

namespace Flowey.Bot
{
    public class EventHandler
    {
        private Afk AfkDb = new Afk(Config.Bot.AirtableApi, Config.Bot.AirtableBaseApi);
        private readonly DiscordSocketClient _Client;
        private readonly CommandService _Commands;
        private readonly IServiceProvider _Service;
        public EventHandler(IServiceProvider Service)
        {
            _Service = Service;
            _Client = _Service.GetService<DiscordSocketClient>();
            _Commands = _Service.GetService<CommandService>();
        }

        public Task InitAsync()
        {
            _Client.Ready += Ready_Event;
            _Client.Log += Client_Log;
            _Commands.Log += Command_Log;
            _Client.MessageReceived += Message_Event;
            _Client.UserJoined += User_Joined_Guild_Event;
            return Task.CompletedTask;
        }

        private async Task User_Joined_Guild_Event(SocketGuildUser user)
        {
            var html = new Welcome(user.Username, user.GetAvatarUrl());
            var channel = _Client.GetChannel(671139806549508117) as SocketTextChannel;
            await channel.SendFileAsync(new MemoryStream(await html.CreateImage()), "welcome.jpg", "");
        }

        private async Task AfkMessage(SocketCommandContext Context, AfkObject afk)
        {
            await Context.Channel.SendMessageAsync($"{Context.Guild.GetUser(afk.Id).Username} is afk: {afk.Message}");
        }

        private async Task TurnOffAfk(SocketCommandContext Context, AfkObject afk)
        {
            var user = Context.User as SocketGuildUser;
            var username = user.Nickname == null ? user.Username : user.Nickname;
            await user.ModifyAsync(x =>
            {
                x.Nickname = username.Replace("[♡]", "");
            });
            afk.IsAfk = false;
            afk.Message = "";
            await AfkDb.UpdateAfk(afk);
            await Context.Channel.SendMessageAsync("I have removed your AFK status.");
        }

        private async Task Message_Event(SocketMessage MessageParam)
        {
            var Message = MessageParam as SocketUserMessage;
            var Context = new SocketCommandContext(_Client, Message);
            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.IsBot) return;
            var mentions = Context.Message.MentionedUsers;
            if(mentions.Count != 0)
            {
                foreach(var user in mentions)
                {
                    bool check = await AfkDb.CheckIfRecordExist(user.Id);
                    if (check)
                    {
                        AfkObject afk = await AfkDb.GetAfk(user.Id);
                        if (afk.IsAfk)
                        {
                            await AfkMessage(Context, afk);
                        }
                    }
                }
            }
            if(await AfkDb.CheckIfRecordExist(Context.User.Id))
            {
                AfkObject afk = await AfkDb.GetAfk(Context.User.Id);
                if (afk.IsAfk)
                {
                    await TurnOffAfk(Context, afk);
                }
            }
            int ArgsPos = 0;
            if (!(Message.HasStringPrefix(Config.Bot.Prefix, ref ArgsPos) || Message.HasMentionPrefix(_Client.CurrentUser, ref ArgsPos))) return;

            var Result = await _Commands.ExecuteAsync(Context, ArgsPos, _Service);
            if(!Result.IsSuccess && Result.Error != CommandError.UnknownCommand)
            {
                Console.WriteLine($"{DateTime.Now} at Command: {_Commands.Search(Context, ArgsPos).Commands[0].Command.Name} in {_Commands.Search(Context, ArgsPos).Commands[0].Command.Module.Name}] {Result.ErrorReason}");
                var embed = new EmbedBuilder();

                embed.WithTitle("***ERROR***");
                embed.WithColor(Color.Red);
                if (Result.Error == CommandError.BadArgCount)
                    embed.WithDescription($"Missing Arguments\nThis commands needs the following arguments:\n{string.Join(", ", _Commands.Search(Context, ArgsPos).Commands[0].Command.Parameters.Select(p => p.Name))}");
                else
                    embed.WithDescription(Result.ErrorReason);

                await Context.Channel.SendMessageAsync("", embed: embed.Build());
            }
        }
        private Task Command_Log(LogMessage arg)
        {
            Console.WriteLine($"{DateTime.Now} => {arg.Source}: {arg.Message}");
            return Task.CompletedTask;
        }

        private Task Client_Log(LogMessage arg)
        {
            Console.WriteLine($"{DateTime.Now} => {arg.Source}: {arg.Message}");
            return Task.CompletedTask;
        }

        private Timer _timer;
        private readonly List<string> _statusList = new List<string>() 
        { 
            "If only… if only we had more time…",
            "I want to know what “I love you” truly means…",
            "'Back then, if we could have heard each others voices, everything would have been so much better.'",
            "Would it be better if we never existed?",
            "What if..there was a world with no suffering?",
            "'No matter how deep the night, it always turns to day, eventually.'",
            "'I want you to be happy. I want you to laugh a lot. I don’t know what exactly I’ll be able to do for you, but I’ll always be by your side.'" 
        };
        private int _statusIndex = 0;
        private async Task Ready_Event()
        {
            await _Client.SetStatusAsync(UserStatus.DoNotDisturb);
            if(_Client.Status != UserStatus.DoNotDisturb)
            _timer = new Timer(async _ =>
            {
                await _Client.SetGameAsync(_statusList.ElementAtOrDefault(_statusIndex), type: ActivityType.Playing);
                _statusIndex = _statusIndex + 1 == _statusList.Count ? 0 : _statusIndex + 1;
                await _Client.SetStatusAsync(UserStatus.Online);
            },
            null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
            else
            {
                await _Client.SetGameAsync("Master is working on me~", type: ActivityType.Playing);
                await _Client.SetStatusAsync(UserStatus.DoNotDisturb);
            }
            Console.WriteLine($"{_Client.CurrentUser.Username} is ready");    
        }
    }
}
