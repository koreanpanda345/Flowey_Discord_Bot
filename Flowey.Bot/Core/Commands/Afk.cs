using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using Flowey.Airtable;
using Flowey.Airtable.Objects;
using Discord.WebSocket;

namespace Flowey.Bot.Core.Commands
{
    public class Afk : ModuleBase<SocketCommandContext>
    {

        private Airtable.Afk Db = new Airtable.Afk(Config.Bot.AirtableApi, Config.Bot.AirtableBaseApi);

        [Command("afk")]
        [Summary("Allows you to go afk.")]
        public async Task AfkCommand([Remainder] string message = "AFK")
        {
            bool check = await Db.CheckIfRecordExist(Context.User.Id);
            if (!check)
                await Db.CreateAfk(new AfkObject
                {
                    Id = Context.User.Id,
                    IsAfk = true,
                    Message = message
                });
            else
            {
                AfkObject afk = await Db.GetAfk(Context.User.Id);
                await Db.UpdateAfk(new AfkObject
                {
                    Id = Context.User.Id,
                    IsAfk = true,
                    Message = message,
                    RecordId = afk.RecordId
                });
            }
            var user = Context.User as SocketGuildUser;
            string username = "";
            if (user.Nickname == null)
                username = user.Username;
            else
                username = user.Nickname;
            await user.ModifyAsync(x =>
            {
                x.Nickname = $"[♡] {username}";
            });
            await Context.Channel.SendMessageAsync($"<a:FLLotusChonkyBellyPats:707464562013896705> I set your afk status to: \"{message}\". Also I'm a very chonku so please giv cuddles<:FLChonkyLotuGibCuddle:711385587139084409>");
        }
    }
}
