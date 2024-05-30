using System.IO.Compression;
using Gml.Plugins.Server.Models;
using Octokit;

namespace Gml.Plugins.Server.Core;

public class PluginsHandler
{
    public static async Task<IResult> GetPlugins(GitHubClient client, PluginProviderCollection plugins)
    {
        if (plugins.Count == 0)
        {
            await plugins.Reload(client);
        }

        return Results.Ok(plugins);
    }
    
    public static async Task<IResult> GetPlugin(string name, string version, GitHubClient client, PluginProviderCollection plugins)
    {
        if (plugins.Count == 0)
        {
            await plugins.Reload(client);
        }

        var plugin = plugins.FirstOrDefault(c => c.Name == name);

        if (plugin is null)
        {
            return Results.NotFound();
        }

        var versionData = plugin.Versions.FirstOrDefault(c => c.Version == version);

        if (versionData is null)
        {
            return Results.NotFound();
        }
        
        var pathToFile = $"{versionData.Folder}.zip";
        var mimeType = "application/octet-stream";
        var fileName = $"plugin-{name}-{version}.zip";

        return Results.File(await File.ReadAllBytesAsync(pathToFile), mimeType, fileName);
    }
}