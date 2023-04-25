using Discord_Bot_Faceit_Stats_Provider.Data;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord_Bot_Faceit_Stats_Provider.Commands
{
    public class DisplayUserElo : BaseCommandModule
    {
        private readonly IHttpClientFactory _httpClientFactory;

        Rootobject playerinf;
        string errorString;

        public DisplayUserElo(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }



        [Command("elo")]

        public async Task CheckElo(CommandContext ctx, string nickname)
        {
            var client = _httpClientFactory.CreateClient("Faceit");

            try
            {
                playerinf = await client.GetFromJsonAsync<Rootobject>($"v4/players?nickname={nickname}");
                errorString = null;

            }
            catch (Exception ex)
            {

                errorString = $"Error: {ex.Message}";
            }

            await ctx.Channel.SendMessageAsync($"{nickname}'s elo is: {playerinf.games.csgo.faceit_elo}");
        }
    }
}


