using Gml.Plugins.Server.Core;
using Gml.Plugins.Server.Models;
using Octokit;

namespace Gml.Plugins.Server.Core;

public class PluginProviderCollection : List<GmlPlugin>
{
    public async Task Reload(GitHubClient client)
    {
        AddRange(await PrepareInstallation.Install(client));
    }
}