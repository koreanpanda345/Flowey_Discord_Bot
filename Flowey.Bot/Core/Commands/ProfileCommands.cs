using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Flowey.Airtable;
using Flowey.Airtable.Objects;
using Discord.Addons.Interactive;


namespace Flowey.Bot.Core.Commands
{
    public class ProfileCommands : InteractiveBase<SocketCommandContext>
    {
        UserProfile UserDb = new UserProfile(Config.Bot.AirtableApi, Config.Bot.AirtableBaseApi);

        [Command("backgroundown")]
        [Alias("bgown")]
        public async Task OwnBackgrounds()
        {
            await Context.Message.DeleteAsync();
            UserObject user = await UserDb.GetUserProfile(Context.User.Id);
            List<string> bgName = new List<string>();
            List<string> bgUrl = new List<string>();
            Console.WriteLine(user.OwnBgNames);
            Console.WriteLine(user.OwnBgUrl);
            for(int i = 0; i < user.OwnBgNames.Split(" ").Length; i++)
            {
                bgName.Add(user.OwnBgNames.Split(" ")[i]);
            }
            for(int i = 0; i < user.OwnBgUrl.Split(" ").Length; i++)
            {
                bgUrl.Add(user.OwnBgUrl.Split(" ")[i]);
            }
            for(int i = 0; i < bgName.Count; i++)
            {
                Console.WriteLine(bgName[i]);
            }
            string desc = "";
            List<string> pages = new List<string>();
            int page = 0;
            int x = 10;
            for (int i = 0; i < (int)Math.Floor(Math.Round((double)bgName.Count / 10)); i++)
            {
                for(int n = 1 + x - 11; n < x; n++)
                {
                    if(n != 0 && !(n > bgName.Count - 1))
                    {
                        desc += $"__**ID: {n}**__ - *{bgName[n - 1]}*\n";
                    }
                    else
                    {
                        desc += "<placeholder>";
                    }
                }
                Console.WriteLine(desc);
                desc = desc.Replace("<placeholder>" , "");
                x += 10;
                pages.Add(desc);
                desc = "";
            }

            var embed = new EmbedBuilder()
            {
                Title = $"These are the backgrounds that you own.",
                Description = pages[page],
            };
            embed.WithFooter($"Page {page + 1} of {pages.Count}");
            var msg = await Context.Channel.SendMessageAsync(embed: embed.Build());
            bool exit = false;
            while (!exit)
            {
                var input = await NextMessageAsync(true, true);
                if (input.Content.ToLower().Equals("next"))
                {
                    if(page != pages.Count - 1)
                    {
                        page++;
                        embed.WithDescription(pages[page]);
                        embed.WithFooter($"Page {page + 1} of {pages.Count}");
                        await msg.ModifyAsync(x =>
                        {
                            x.Embed = embed.Build();
                        });
                    }
                }
                else if (input.Content.ToLower().Equals("back"))
                {
                    if(page != 0)
                    {
                        page--;
                        embed.WithDescription(pages[page]);
                        embed.WithFooter($"Page {page + 1} of {pages.Count}");
                        await msg.ModifyAsync(x =>
                        {
                            x.Embed = embed.Build();
                        });
                    }
                }
                else if (input.Content.ToLower().Equals("cancel"))
                {
                    await msg.DeleteAsync();
                    exit = true;
                }
                else { }
                await input.DeleteAsync();
            }
        }
        [Command("backgroundset")]
        [Alias("bgset")]
        [Summary("sets the background of your profile.")]
        public async Task SetBackground(string id)
        {
            UserObject user = await UserDb.GetUserProfile(Context.User.Id);
            List<string> bgName = new List<string>();
            List<string> bgUrl = new List<string>();
            for (int i = 0; i < user.OwnBgNames.Split(" ").Length; i++)
            {
                bgName.Add(user.OwnBgNames.Split(" ")[i]);
            }
            for (int i = 0; i < user.OwnBgUrl.Split(" ").Length; i++)
            {
                bgUrl.Add(user.OwnBgUrl.Split(" ")[i]);
            }
            int _id = Convert.ToInt32(id);
            if (_id > bgName.Count || _id - 1 < 0)
            {
                var __msg = await Context.Channel.SendMessageAsync($"Sorry, but it looks like that id doesn't exist.");
                await Context.Message.DeleteAsync();
                await __msg.DeleteAsync(new RequestOptions() { Timeout = new DateTime().AddMinutes(1).Millisecond});

                return;
            }

            var embed = new EmbedBuilder()
            {
                Title = $"Setting Background",
                Description = $"You are sure you want to change your background to {bgName[_id - 1]}?",
                ImageUrl = bgUrl[_id - 1]
            };

            var msg = await Context.Channel.SendMessageAsync(embed: embed.Build());
            var input = await NextMessageAsync(true, true);
            if (input.Content.ToLower().Equals("yes"))
            {
                user.Background = bgUrl[_id - 1];
                await UserDb.UpdateUserProfile(user);
                var _msg = await Context.Channel.SendMessageAsync($"Ok, I setted your background to be {bgName[_id - 1]}");
                await msg.DeleteAsync();
                await input.DeleteAsync();
                await _msg.DeleteAsync(new RequestOptions() { Timeout = new DateTime().AddMinutes(1).Millisecond});
            }
            else if (input.Content.ToLower().Equals("no"))
            {
                var _msg = await Context.Channel.SendMessageAsync($"Ok, I won't change your background.");
                await msg.DeleteAsync();
                await input.DeleteAsync();
                await _msg.DeleteAsync(new RequestOptions() { Timeout = new DateTime().AddMinutes(1).Millisecond });
            }
        }
        [Command("birthday")]
        [Alias("setbirthday", "birthdayset", "bday", "bdayset", "setbday")]
        public async Task SetBirthday(string birthday)
        {
            if(!await UserDb.CheckIfRecordExist(Context.User.Id))
            {
                await UserDb.CreateUserProfile(new UserObject()
                {
                    Id = Context.User.Id
                });
            }
            UserObject user = await UserDb.GetUserProfile(Context.User.Id);
            user.Birthday = birthday;
            await UserDb.UpdateUserProfile(user);
            await Context.Channel.SendMessageAsync($"Updated your birthday to {birthday}");
        }
    }
}
