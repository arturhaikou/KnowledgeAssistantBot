using BotsDemo.KnowledgeAssistant.Bots;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.KernelMemory;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddTransient(sp => new MemoryWebClient(builder.Configuration.GetValue<string>("KernelMemory:Host")));
builder.Services.AddTransient<IBot, KnowledgeAssistantAIBot>();
builder.Services.AddSingleton<IBotFrameworkHttpAdapter>(sp => new CloudAdapter(builder.Configuration));
builder.Services.AddHttpLogging();
var app = builder.Build();

app.UseHttpLogging();
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/api/messages", async (IBot bot, IBotFrameworkHttpAdapter adapter, HttpContext context) 
    => await adapter.ProcessAsync(context.Request, context.Response, bot));

app.Run();
