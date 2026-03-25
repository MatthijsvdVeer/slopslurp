using SlopSlurp.Rules;
using SlopSlurp.Rules.LlmPowered;
using SlopSlurp.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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
