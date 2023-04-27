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
using static Discord_Bot_Faceit_Stats_Provider.Data.LastMatch;

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
            Random random2 = new Random();

            int EloToLoseOrWin = random.Next(0, 201);
            int WinOrLose = random2.Next(0,2);

            if (WinOrLose == 0)
            {
                if (EloToLoseOrWin >= 100)
                {
                    await ctx.Channel.SendMessageAsync($"Ile elo dziś przepierdolisz? A tak ze {EloToLoseOrWin}");
                }
                else
                {
                    await ctx.Channel.SendMessageAsync($"Ile elo dziś przepierdolisz? A tak z {EloToLoseOrWin}");
                }
            }

            else 
            {
                    await ctx.Channel.SendMessageAsync($"Ile elo dziś przepierdolisz? Dziś tylko wygrywasz stary, {EloToLoseOrWin} elo na +");
            }
        }

        //Reivil

        [Command("Reivil")]

        public async Task ReivilBot(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("14 avg bot w twojej okolicy, kliknij w link \n" +
                "https://www.youtube.com/watch?v=i7Bsp1e0tNw&ab_channel=Reivil");
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

            if (playerinf != null)
            {
                StringBuilder statsBuilder = new StringBuilder();

                statsBuilder.AppendLine($"{playerinf.nickname}'s last match stats: ");

                foreach (var round in lastmatch.rounds)
                {
                    var playerStats = round.teams.SelectMany(t => t.players).FirstOrDefault(p => p.nickname == playerinf.nickname)?.player_stats;

                    if (playerStats != null)
                    {
                        decimal Kills = Decimal.Parse(playerStats.Kills);

                        decimal Deaths = Decimal.Parse(playerStats.Deaths);

                        decimal RoundsPlayed = Decimal.Parse(round.round_stats.Rounds);

                        //decimal Assists = Decimal.Parse(playerStats.Assists);

                        decimal KdRatio = Kills / Deaths;

                        decimal KrRatio = Kills / RoundsPlayed;

                        //decimal SurvivedRounds = (RoundsPlayed - Deaths) * 100;

                        //decimal Impact = (Kills + Assists - Deaths + 4 + 0.5M * 4) / (16 + 8);

                        //decimal KillsDamage = Kills * 100;

                        //decimal AssistsDamage = Assists * 50;

                        //decimal ADR = (KillsDamage + AssistsDamage) / RoundsPlayed;

                        //decimal DeathsPerRound = Deaths/RoundsPlayed;

                        //decimal KAST = (Kills + Assists + SurvivedRounds) / RoundsPlayed-5;

                        //decimal HLTVrating = (0.405022M * KrRatio) - (0.657678M * DeathsPerRound) + (0.524246M * KAST) / 100 + Impact / 5 + (0.00410341M * ADR) + 0.343334M;

                        string result = string.Empty;

                        if (playerStats.Result == 1.ToString())
                        {
                            result = "Win";
                        }
                        else
                        {
                            result = "Lose";
                        }

                        statsBuilder.AppendLine($"Result: {result}, Score: {round.round_stats.Score}, Kills: {playerStats.Kills}, Deaths: {playerStats.Deaths}, Assists: {playerStats.Assists}, K/D Ratio: {KdRatio.ToString("F2")} K/R Ratio: {KrRatio.ToString("F2")}");
                    }
                }

                await ctx.Channel.SendMessageAsync(statsBuilder.ToString());

                playerinf = null;
            }

            else
            {
                await ctx.Channel.SendMessageAsync("There is no such a player in the database.");
            }
        }
    }
}


