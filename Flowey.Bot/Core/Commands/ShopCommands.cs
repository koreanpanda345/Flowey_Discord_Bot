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
using System.Linq;

namespace Flowey.Bot.Core.Commands
{
    public class ShopCommands : InteractiveBase<SocketCommandContext>
    {
        Shop shop = new Shop(Config.Bot.AirtableApi, Config.Bot.AirtableBaseApi);
        UserProfile UserDb = new UserProfile(Config.Bot.AirtableApi, Config.Bot.AirtableBaseApi);


        [Command("background")]
        [Alias("bg", "backgrounds")]
        [Summary("")]
        public async Task Background(string options = "shop", [Remainder] string args = "")
        {
            switch (options)
            {
                case "shop":
                    await ShopBackgrounds();
                break;
                case "buy":
                    await BuyBackground(args);
                    break;
                case "view":
                    await ViewBackground(args);
                    break;
            }
        }

        public async Task ViewBackground(string id)
        {
            List<BackgroundObject> backgrounds = await shop.GetBackgrounds();
            int _id = Convert.ToInt32(id) - 1;
            if (_id < 0 || _id > backgrounds.Count - 1) return;
            BackgroundObject background = backgrounds[_id];
            var embed = new EmbedBuilder()
            {
                Title = $"{background.Name}",
                Description = $"Price: {(background.Dev == true ? "Developer" : background.Nitro == true ? "Nitro Boosting" : background.Price == 0 ? "Free" : background.Price.ToString())}",
                ImageUrl = background.Url,
                Color = Color.Teal
            };

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }

        public async Task BuyBackground(string id)
        {
            if (await UserDb.CheckIfRecordExist(Context.User.Id))
            {
                await UserDb.CreateUserProfile(new UserObject
                {
                    Id = Context.User.Id
                });
            }
            
            UserObject user = await UserDb.GetUserProfile(Context.User.Id);

            List<BackgroundObject> backgrounds = await shop.GetBackgrounds();
            int _id = Convert.ToInt32(id) - 1;
            if(_id == 0 || _id > (backgrounds.Count - 1))
            {
                await Context.Channel.SendMessageAsync($"Sorry, but it looks like there is no background under the id of {_id}");
                return;
            }
            var _user = Context.User as SocketGuildUser;
            BackgroundObject background = backgrounds[_id];

            if(user.OwnBgNames.Split(" ").Contains(background.Name))
            {
                await Context.Channel.SendMessageAsync($"It looks like you already own this background.");
                return;
            }
            if (background.Dev)
            {
                SocketRole role = Context.Guild.GetRole(673572050945966130);
                if (!_user.Roles.Contains(role))
                {
                    await Context.Channel.SendMessageAsync($"Sorry, but this is Bot Developer exclusives.");
                    return;
                }
                user.OwnBgNames += " " + background.Name;
                user.OwnBgUrl += " " + background.Url;
                await UserDb.UpdateUserProfile(user);
            }
            else if (background.Nitro)
            {
                SocketRole role = Context.Guild.GetRole(586439609085329429);
                if (!_user.Roles.Contains(role))
                {
                    await Context.Channel.SendMessageAsync($"Sorry, but this is a Nitro Booster's exclusives. You must boost the server to gain access to this background.");
                    return;
                }
                user.OwnBgNames += " " + background.Name;
                user.OwnBgUrl += " " + background.Url;
                await UserDb.UpdateUserProfile(user);
            }
            else
            {
                if (user.Balance < background.Price)
                {
                    await Context.Channel.SendMessageAsync($"Sorry, but it looks like you do not have enough flowers to make this purchase. you need {background.Price - user.Balance} flowers more.");
                    return;
                }
                user.Balance -= background.Price;
                user.OwnBgNames += " " + background.Name;
                user.OwnBgUrl += " " + background.Url;
                await UserDb.UpdateUserProfile(user);
            }

            await Context.Channel.SendMessageAsync($"You successfully bought {background.Name}");
        }

        public async Task ShopBackgrounds()
        {
            await Context.Message.DeleteAsync();
            var backgrounds = await shop.GetBackgrounds();
            string desc = "";
            List<string> pages = new List<string>();
            int page = 0;
            int x = 1;
            int i = 1;
            foreach(var background in backgrounds)
            {
               if(x != (5 * i))
                {
                    desc += $"__**ID: {x}**__ *{background.Name}* Price: {(background.Dev == true ? "Dev" : background.Nitro == true ? "Nitro" : background.Price.ToString())}\n";
                }
                else
                {
                    pages.Add(desc);
                    desc = $"__**ID: {x}**__ *{background.Name}* Price: {(background.Dev == true ? "Dev" : background.Nitro == true ? "Nitro" : background.Price.ToString())}\n";
                    i++;
                }
                x++;
            }
            var embed = new EmbedBuilder()
            {
                Title = $"These are the avaiable backgrounds."
            };
            embed.WithDescription(pages[page]);

            embed.WithFooter($"Page {page} of {pages.Count}");

            var msg = await Context.Channel.SendMessageAsync(embed: embed.Build());
            bool exit = false;
            while (!exit)
            {
                var input = await NextMessageAsync(true, true);
                if (input.Content.ToLower().Equals("next"))
                {
                    if (page != pages.Count)
                    {
                        page++;
                        embed.WithDescription(pages[page]);
                        embed.WithFooter($"Page {page} of {pages.Count}");
                        await msg.ModifyAsync(x =>
                        {
                            x.Embed = embed.Build();
                        });
                    }
                }
                else if (input.Content.ToLower().Equals("back"))
                {
                    if (page != 0)
                    {
                        page--;
                        embed.WithDescription(pages[page]);
                        embed.WithFooter($"Page {page} of {pages.Count}");
                        await msg.ModifyAsync(x =>
                        {
                            x.Embed = embed.Build();
                        });
                    }
                }
                else if (input.Content.ToLower().Equals("cancel"))
                {
                    msg.DeleteAsync();
                    
                    exit = true;
                }
                else { }

                await input.DeleteAsync();
            }
        }
    }
}
