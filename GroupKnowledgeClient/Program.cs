using GroupKnowledgeClient;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using GroupKnowledgeClient.State;
using SampleState = GroupKnowledgeClient.State.SampleData;
using GroupKnowledgeClient.Services;
using SampleData = GroupKnowledgeClient.Services.SampleData;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddMudBlazorSnackbar();
builder.Services.AddSingleton<ILocalStorage, SampleData.LocalStorage>();
builder.Services.AddSingleton<IGroupState, SampleState.GroupState>();
builder.Services.AddSingleton<IWalletState, SampleState.WallatState>();

await builder.Build().RunAsync();
