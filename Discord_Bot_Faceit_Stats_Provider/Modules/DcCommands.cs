using Discord_Bot_Faceit_Stats_Provider.Data;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
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
using DSharpPlus.Entities;
using System.Threading.Tasks;
using static Discord_Bot_Faceit_Stats_Provider.Data.LastMatch;
using System.IO;
using System.Runtime.Versioning;
using System.Reflection;

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
            Random random = new Random();

            int option = random.Next(0, 2);

            if (option == 1)
            {
                await ctx.Channel.SendMessageAsync("14 avg bot w twojej okolicy, kliknij w link \n" +
                    "https://www.youtube.com/watch?v=i7Bsp1e0tNw&ab_channel=Reivil");
            }

            else 
            {
                FileStream file = new FileStream(@"E:\Desktop\Reivilstats.png", FileMode.Open);
                DiscordMessageBuilder messagefile = new DiscordMessageBuilder();
                messagefile.AddFile(file);
                await ctx.Channel.SendMessageAsync("xD");
                await ctx.Channel.SendMessageAsync(messagefile);
                file.Close();
            }
        }

        //lastmatch

        [Command("lastmatch")]

        public async Task LastMatchStats(CommandContext ctx,string nickname)
        {
            var client = _httpClientFactory.CreateClient("Faceit");

            try
            {
                playerinf = await client.GetFromJsonAsync<BasicPlayerInformations.Rootobject>($"v4/players?nickname={nickname}");
                matcheshistory = await client.GetFromJsonAsync<MatchesHistory.Rootobject>($"v4/players/{playerinf.player_id}/history?game=csgo&offset=0&limit=5");
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

                        decimal KdRatio = Kills / Deaths;

                        decimal KrRatio = Kills / RoundsPlayed;

                        string result = string.Empty;

                        if (playerStats.Result == 1.ToString())
                        {
                            result = "Win";
                        }
                        else
                        {
                            result = "Lose";
                        }

                        statsBuilder.AppendLine($"Result: {result},  Score: {round.round_stats.Score},  Kills: {playerStats.Kills},  Deaths: {playerStats.Deaths},  Assists: {playerStats.Assists},  K/D Ratio: {KdRatio.ToString("F2")},  K/R Ratio: {KrRatio.ToString("F2")}");
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

        [Command("last5")]

        public async Task Last5(CommandContext ctx, string nickname)
        {
            var client = _httpClientFactory.CreateClient("Faceit");
            List<LastMatch.Rootobject> fiveLastMatches = new List<LastMatch.Rootobject>();

            try
            {
                playerinf = await client.GetFromJsonAsync<BasicPlayerInformations.Rootobject>($"v4/players?nickname={nickname}");
                matcheshistory = await client.GetFromJsonAsync<MatchesHistory.Rootobject>($"v4/players/{playerinf.player_id}/history?game=csgo&offset=0&limit=5");

            }
            catch (Exception ex)
            {
                errorString = $"Error: {ex.Message}";
            }

            if (playerinf != null)
            {
                for (int i = 0; i < matcheshistory.items.Count(); i++)
                {
                    var match = await client.GetFromJsonAsync<LastMatch.Rootobject>($"v4/matches/{matcheshistory.items[i].match_id}/stats");
                    fiveLastMatches.Add(match);
                }

                decimal SingleMatchKills = 0;
                decimal SingleMatchDeaths = 0;
                decimal SingleMatchKd = 0;
                decimal SingleMatchKr = 0;
                decimal totalkills = 0;
                decimal totalDeaths = 0;
                decimal totalKd = 0;
                decimal totalKr = 0;

                foreach (var match in fiveLastMatches)
                {
                    foreach (var round in match.rounds)
                    {
                        foreach (var team in round.teams)
                        {
                            foreach (var player in team.players)
                            {
                                if (player.nickname == nickname)
                                {
                                    SingleMatchKills = decimal.Parse(player.player_stats.Kills);
                                    SingleMatchDeaths = decimal.Parse(player.player_stats.Deaths);
                                    SingleMatchKd = SingleMatchKills / SingleMatchDeaths;
                                    SingleMatchKr = SingleMatchKills / decimal.Parse(round.round_stats.Rounds);
                                    totalKd += SingleMatchKd;
                                    totalKr += SingleMatchKr;
                                    totalkills += SingleMatchKills;
                                    totalDeaths += SingleMatchDeaths;
                                }
                            }
                        }
                    }
                }

                decimal averageKd = totalKd / matcheshistory.items.Count();
                decimal averageKr = totalKr / matcheshistory.items.Count();
                decimal averageKills = Math.Floor(totalkills / matcheshistory.items.Count());
                decimal averageDeaths = Math.Floor(totalDeaths / matcheshistory.items.Count());

                StringBuilder statsBuilder = new StringBuilder();

                statsBuilder.AppendLine($"{nickname}'s last 5 match stats: ");
                statsBuilder.AppendLine($"Average Kills: {averageKills}  Average Deaths: {averageDeaths}  K/D: {averageKd:F2}  K/R {averageKr:F2}");

                await ctx.RespondAsync(statsBuilder.ToString());

                playerinf = null;
                matcheshistory = null;
            }

            else
            {
                await ctx.Channel.SendMessageAsync("There is no such a player in the database. Please try again.");
            }
        }
    }
}


