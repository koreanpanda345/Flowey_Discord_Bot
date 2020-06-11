using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Flowey.Airtable.Objects;
using Flowey.Airtable;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;

namespace Flowey.Bot.Core.Commands
{
    public class Economics : ModuleBase<SocketCommandContext>
    {
        private UserProfile UserDb = new UserProfile(Config.Bot.AirtableApi, Config.Bot.AirtableBaseApi);
        private Cooldowns cooldown = new Cooldowns(Config.Bot.AirtableApi, Config.Bot.AirtableBaseApi);

        [Command("balance")]
        [Alias("bal")]
        [Summary("Displays your current balance")]
        public async Task BalanceCommand()
        {
            if(await UserDb.CheckIfRecordExist(Context.User.Id))
            {
                await UserDb.CreateUserProfile(new UserObject()
                {
                    Id = Context.User.Id,
                });
            }

            UserObject user = await UserDb.GetUserProfile(Context.User.Id);
            var embed = new EmbedBuilder()
            {
                Color = new Color(0xffd1dc),
                Title = $"{Context.User.Username}'s Flower Garden",
                Description = $"You currently have {user.Balance} <a:FLflower2:678622764080431114> flowers in your garden!"
            };

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }


        [Command("daily")]
        [Summary("Gives you flowers once a day.")]
        public async Task DailyCommand()
        {
            int amount = 1000;
            if (await cooldown.CheckIfRecordExist(Context.User.Id))
            {
                CooldownObject userCooldowns = await cooldown.GetCooldowns(Context.User.Id);
                Console.WriteLine(userCooldowns.Daily);
                if (Convert.ToInt64(DateTime.Now.Millisecond) <= userCooldowns.Daily)
                {
                    var time = userCooldowns.Daily - Convert.ToInt64(DateTime.Now.Millisecond);
                    Console.WriteLine(time);
                    await Context.Channel.SendMessageAsync($"Sorry, but you are still on a cooldown for this command. Please wait another `{(new DateTime(time).Hour == 1 ? $"{new DateTime(time).Minute} minutes" : $"{new DateTime(time).Hour} hours")}`");
                    return;
                }
            }
            if (!await UserDb.CheckIfRecordExist(Context.User.Id))
                await UserDb.CreateUserProfile(new UserObject()
                {
                    Id = Context.User.Id,
                    Balance = amount
                });
            else
            {
                UserObject user = await UserDb.GetUserProfile(Context.User.Id);
                user.Balance = user.Balance + amount;
                await UserDb.UpdateUserProfile(user);
            }
            var embed = new EmbedBuilder()
            {
                Color = new Color(0xffb6c1),
                Title = $"{Context.User.Username}'s Daily Reward!!!",
                Description = $"{Context.User.Username} got {amount} <a:FLflower2:678622764080431114> flowers as their daily reward!"
            };
            CooldownObject _userCooldowns = await cooldown.GetCooldowns(Context.User.Id);
            _userCooldowns.Daily = _userCooldowns.Daily + 86400000; // 86400000 milliseconds = 1 day
            await cooldown.UpdateCooldowns(_userCooldowns);
            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }
    }
}
