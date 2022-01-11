using GroupKnowledgeClient;
using Default = GroupKnowledgeClient.State.Default;
using Fake = GroupKnowledgeClient.Services.Fake;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using GroupKnowledgeClient.State;
using GroupKnowledgeClient.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddSingleton<IGroupState, Default.GroupState>();
builder.Services.AddSingleton<IWalletState, Default.WalletState>();
builder.Services.AddSingleton<ILocalStorage, Fake.LocalStorage>();
builder.Services.AddSingleton<IBlockchain, Fake.Blockchain>();

await builder.Build().RunAsync();
