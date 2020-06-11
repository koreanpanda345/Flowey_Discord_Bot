using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Flowey.Bot.Core.Commands
{
    public class Reactions : ModuleBase<SocketCommandContext>
    {
        HttpClient client = new HttpClient();
        
        private async Task<Object> GetNekoLifeImage(string type)
        {
            object body = null;
            client.BaseAddress = new Uri("https://nekos.life/");
            HttpResponseMessage response = await client.GetAsync($"api/v2/img/{type}");
            if (response.IsSuccessStatusCode)
            {
                body = await response.Content.ReadAsStringAsync();
            }
            Console.WriteLine(body);
            return body;
        }

        public async Task AbuseCommand(string user)
        {
            List<string> imgSource = new List<string>();
            imgSource.Add("https://thumbs.gfycat.com/EllipticalLargeKitfox-size_restricted.gif");
            imgSource.Add("https://data.whicdn.com/images/301885728/original.gif");
            imgSource.Add("https://ci.memecdn.com/4158473.gif");
            imgSource.Add("https://media1.tenor.com/images/c4f16143fd2df930ad3e64a3f902d486/tenor.gif?itemid=12388326");
            imgSource.Add("https://media.giphy.com/media/t2rkjXOxuptra/giphy.gif");
            var users = Context.Message.MentionedUsers;
            
            var _user = users as SocketGuildUser;
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = $"**{Context.User.Username}** is abusing **{_user.Username}**' h-harder senpai~",
                ImageUrl = $"{imgSource[RandomInt(0, imgSource.Count)]}",
                Color = new Color(0xffd1dc),
            
            };

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }


        private int RandomInt(int min, int max)
        {
            Random rand = new Random();
            return rand.Next(min, max);
        }
    }
}
