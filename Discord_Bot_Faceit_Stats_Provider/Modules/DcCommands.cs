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
    public class DcCommands : BaseCommandModule
    {
        private readonly IHttpClientFactory _httpClientFactory;

        BasicPlayerInformations.Rootobject playerinf;
        MatchesHistory.Rootobject matcheshistory;
        LastMatch.Rootobject lastmatch;

        string errorString;

        public DcCommands(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        //elo

        [Command("elo")]

        public async Task CheckElo(CommandContext ctx, string nickname)
        {
            var client = _httpClientFactory.CreateClient("Faceit");

            try
            {
                playerinf = await client.GetFromJsonAsync<BasicPlayerInformations.Rootobject>($"v4/players?nickname={nickname}");
                errorString = null;

            }
            catch (Exception ex)
            {

                errorString = $"Error: {ex.Message}";
            }

            if (errorString!=null)
            {
                await ctx.Channel.SendMessageAsync("There is no such a player in the database.");
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"{nickname}'s elo is: {playerinf.games.csgo.faceit_elo}");
            }
        }

        //ile

        [Command("ile")]

        public async Task HowManyEloDoIlose(CommandContext ctx)
        {
            Random random = new Random();

            int EloToLose = random.Next(0, 251);

            if (EloToLose >= 100)
            {
                await ctx.Channel.SendMessageAsync($"Ile elo dziś przepierdolisz? A tak ze {EloToLose}");
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"Ile elo dziś przepierdolisz? A tak z {EloToLose}");
            }
        }

        //Reivil

        [Command("Reivil")]

        public async Task ReivilBot(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("14 avg bot");
        }

        //lastmatch

        [Command("lastmatch")]

        public async Task LastMatchStats(CommandContext ctx,string nickname)
        {
            var client = _httpClientFactory.CreateClient("Faceit");

            try
            {
                playerinf = await client.GetFromJsonAsync<BasicPlayerInformations.Rootobject>($"v4/players?nickname={nickname}");
                matcheshistory = await client.GetFromJsonAsync<MatchesHistory.Rootobject>($"v4/players/{playerinf.player_id}/history?game=csgo&offset=0&limit=1");
                lastmatch = await client.GetFromJsonAsync<LastMatch.Rootobject>($"v4/matches/{matcheshistory.items[0].match_id}/stats");
                errorString = null;

            }
            catch (Exception ex)
            {

                errorString = $"Error: {ex.Message}";
            }

            StringBuilder statsBuilder = new StringBuilder();

            statsBuilder.AppendLine($"{playerinf.nickname}'s last match stats: ");

            foreach (var round in lastmatch.rounds)
            {
                var playerStats = round.teams.SelectMany(t => t.players).FirstOrDefault(p => p.nickname == playerinf.nickname)?.player_stats;
                if (playerStats != null)
                {
                    decimal KdRatio = (Decimal.Parse(playerStats.Kills) / Decimal.Parse(playerStats.Deaths));
                    statsBuilder.AppendLine($"Kills: {playerStats.Kills}, Deaths: {playerStats.Deaths}, Assists: {playerStats.Assists}, K/D Ratio: {KdRatio.ToString("F2")}, K/R Ratio: {playerStats.K_R_Ratio}");
                }
            }      

            await ctx.Channel.SendMessageAsync(statsBuilder.ToString());
        }
    }
}


