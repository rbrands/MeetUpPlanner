using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BlazorDownloadFile;
using Blazored.LocalStorage;
using CurrieTechnologies.Razor.Clipboard;
using MeetUpPlanner.Shared;

namespace MeetUpPlanner.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSingleton<AppState>();
            builder.Services.AddSingleton<KeywordCheck>();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddTransient<BlazorTimer>();
            builder.Services.AddBlazorDownloadFile();
            builder.Services.AddClipboard();
            await builder.Build().RunAsync();
        }
    }
}
