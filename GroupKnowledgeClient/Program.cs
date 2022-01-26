using Blazored.LocalStorage;
using GroupKnowledgeClient;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using GroupKnowledgeClient.State;
using GroupKnowledgeClient.Services;
using DefaultData = GroupKnowledgeClient.Services.Default;
using SampleData = GroupKnowledgeClient.Services.SampleData;
using DefaultState = GroupKnowledgeClient.State.Default;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddMudBlazorSnackbar();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IGroupState, DefaultState.GroupState>();
builder.Services.AddScoped<IAgentState, DefaultState.AgentState>();

await builder.Build().RunAsync();
