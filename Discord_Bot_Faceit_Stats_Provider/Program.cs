using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Discord_Bot_Faceit_Stats_Provider.Commands;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using System.ComponentModel.Design;
using DSharpPlus.CommandsNext;

namespace Discord_Bot_Faceit_Stats_Provider
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Path.Combine(AppContext.BaseDirectory, "..", ".."))
                            .AddJsonFile("config.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            var host = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHttpClient("Faceit", httpClient =>
                    {
                        httpClient.BaseAddress = new Uri(configuration.GetValue<string>("FaceitURI"));
                        httpClient.DefaultRequestHeaders.Add("Authorization", configuration.GetValue<string>("FaceitAPI"));
                    });

                    services.AddSingleton<Bot>();
                    services.AddTransient<DisplayUserElo>();

                })
                .Build();

            var BotService = host.Services.GetRequiredService<Bot>();
           

            await BotService.RunAsync();
        }
    }
}
