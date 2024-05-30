using Gml.Plugins.Server.Core;
using Octokit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<PluginProviderCollection>();
builder.Services.AddSingleton<GitHubClient>(serviceProvider =>
{
    var productInformation = new ProductHeaderValue(Environment.GetEnvironmentVariable("Application"));
    var credentials = new Credentials(Environment.GetEnvironmentVariable("GitHubToken"));
    return new GitHubClient(productInformation) { Credentials = credentials };
});

// Adding CORS service and defining a policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

await PrepareInstallation.Install(app.Services.GetRequiredService<GitHubClient>());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Applying the CORS policy to the app
app.UseCors("AllowAllOrigins");

app.RegisterEndpoints();

app.Run();