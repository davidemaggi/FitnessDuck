using System.Globalization;
using System.Text;
using Blazored.LocalStorage;
using FitnessData.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FitnessDuck.Client;
using FitnessDuck.Client.Handlers;
using FitnessDuck.Client.Services;
using FitnessDuck.Core.Services.Implementations;
using FitnessDuck.Share.Clients.Implementations;
using FitnessDuck.Share.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Translations;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");



builder.Services.AddScoped<TokenService>();

builder.Services.AddScoped<AuthenticatedHttpClientHandler>();
builder.Services.AddSingleton<ICalendardService,CalendarService>();





builder.Services.AddHttpClient<AuthApiClient>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5235/");
    })
    .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

builder.Services.AddHttpClient<LessonApiClient>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5235/");
    })
    .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

builder.Services.AddMudServices();

builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<JwtAuthenticationStateProvider>());


builder.Services.AddMudTranslations();
builder.Services.AddLocalization();

// Register MudBlazor localization to use your ResX-based localizer
builder.Services.AddTransient<MudLocalizer, ResXMudLocalizer>();

var host = builder.Build();

var js = host.Services.GetRequiredService<IJSRuntime>();

// Get browser culture via JS interop
var result = await js.InvokeAsync<string>("blazorCulture.get");

CultureInfo? culture;

if (result != null)
{
    culture = new CultureInfo(result);
}
else
{
    // fallback if no culture saved in browser
    culture = new CultureInfo("it-it");  // Or your desired default culture (change this)
}

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;



await host.RunAsync();





System.AppDomain.CurrentDomain.FirstChanceException += (sender, error) =>
{
    System.Console.WriteLine("!!! Encountered exception !!!");
};