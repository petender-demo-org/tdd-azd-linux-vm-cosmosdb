using Azure.Identity;
using LinuxVmCosmosdb.Web.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var cosmosEndpoint = builder.Configuration["COSMOS_ENDPOINT"]
    ?? Environment.GetEnvironmentVariable("COSMOS_ENDPOINT")
    ?? throw new InvalidOperationException("COSMOS_ENDPOINT is not configured.");

builder.Services.AddSingleton(_ =>
{
    var credential = new DefaultAzureCredential();
    return new CosmosClient(cosmosEndpoint, credential, new CosmosClientOptions
    {
        SerializerOptions = new CosmosSerializationOptions
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        }
    });
});

builder.Services.AddSingleton<CosmosDbService>();
builder.Services.AddHealthChecks().AddCheck<CosmosDbHealthCheck>("cosmosdb");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.MapHealthChecks("/healthz");

// Seed data on startup
using (var scope = app.Services.CreateScope())
{
    var cosmosService = scope.ServiceProvider.GetRequiredService<CosmosDbService>();
    await cosmosService.InitializeAsync();
}

app.Run();
