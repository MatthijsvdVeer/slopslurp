using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using SlopSlurp.Observability;
using SlopSlurp.Rules;
using SlopSlurp.Rules.LlmPowered;
using SlopSlurp.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add OpenTelemetry observability
builder.AddTelemetry("web");

// Register IChatClient via Azure OpenAI / Foundry
var endpoint = builder.Configuration["Foundry:Endpoint"]
    ?? throw new InvalidOperationException("Foundry:Endpoint configuration is required.");
var model = builder.Configuration["Foundry:Model"]
    ?? throw new InvalidOperationException("Foundry:Model configuration is required.");

builder.Services.AddChatClient(
    new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
        .GetChatClient(model)
        .AsIChatClient());

// Register SlopSlurp services
builder.Services.AddSingleton<TropeAnalysisAgent>();
builder.Services.AddScoped<ValidationEngine>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
