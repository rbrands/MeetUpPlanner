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
using Radzen;
using MeetUpPlanner.Shared;

namespace MeetUpPlanner.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSingleton<AppState>();
            builder.Services.AddSingleton<KeywordCheck>();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddTransient<BlazorTimer>();
            builder.Services.AddBlazorDownloadFile();
            builder.Services.AddClipboard();
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            await builder.Build().RunAsync();
        }
    }
}
