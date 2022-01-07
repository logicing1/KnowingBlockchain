using GroupKnowledgeClient;
using GroupKnowledgeClient.Services;
using Default = GroupKnowledgeClient.Services.Default;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddSingleton<IGroupsService, Default.GroupsService>();
builder.Services.AddSingleton<IWalletService, Default.WalletService>();
builder.Services.AddSingleton<IBreadcrumbService, Default.BreadcrumbService>();

await builder.Build().RunAsync();
