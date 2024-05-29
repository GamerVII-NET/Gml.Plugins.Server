using System.IO.Compression;
using Gml.Plugins.Server.Core;
using Gml.Plugins.Server.Models;
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

var app = builder.Build();

await PrepareInstallation.Install(app.Services.GetRequiredService<GitHubClient>());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.RegisterEndpoints();
app.UseHttpsRedirection();

app.Run();

public class PluginProviderCollection : List<GmlPlugin>
{
    public async Task Reload(GitHubClient client)
    {
        AddRange(await PrepareInstallation.Install(client));
    }
}