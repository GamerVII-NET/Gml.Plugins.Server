using System.IO.Compression;
using Gml.Plugins.Server.Models;
using Octokit;

namespace Gml.Plugins.Server.Core;

public class PrepareInstallation
{
    private static readonly List<string> BlackList =
    [
        ".gitmodules",
        "README.md"
    ];


    public static async Task<IEnumerable<GmlPlugin>> Install(GitHubClient client)
    {
        var plugins = new List<GmlPlugin>();
        Console.WriteLine("[Gml] Получение списка файлов");

        var result = await client.Repository.Content.GetAllContents("GamerVII-NET", "Gml.Backend.Plugins");

        var submodules = result.Where(c => !BlackList.Contains(c.Name));

        var originalRepository = submodules
            .Select(c => c.HtmlUrl.Split("/tree").First())
            .Select(c => new Uri(c))
            .Select(c => new
            {
                Owner = c.AbsolutePath.Trim('/').Split('/')[0],
                Repository = c.AbsolutePath.Trim('/').Split('/')[1]
            })
            .Select(c => client.Repository.Get(c.Owner, c.Repository).Result)
            .Where(c => c != null)
            .ToList();

        Console.WriteLine($"[Gml] Получено плагинов: {originalRepository.Count}");
        var processedCount = 1;

        foreach (var repository in originalRepository)
        {
            var plugin = new GmlPlugin
            {
                Name = repository.Name,
                Author = repository.Owner.Login,
                Description = repository.Description,
                Homepage = repository.HtmlUrl,
                License = repository.License?.Name,
                Stars = repository.StargazersCount,
            };

            var releases = await client.Repository.Release.GetAll(repository.Id);

            foreach (var release in releases)
            {
                
                string versionFolderPath =
                    $"PluginReleases/{repository.Owner.Login}-{repository.Name}/{release.TagName}";
                
                plugin.Versions.Add(new GmlVersion
                {
                    Version = release.TagName,
                    Name = release.Name,
                    Body = release.Body,
                    Folder = Path.Combine(Environment.CurrentDirectory, versionFolderPath)
                });

                if (File.Exists($"{versionFolderPath}.zip"))
                {
                    continue;
                }

                Console.WriteLine($"[Gml] [{processedCount}/{originalRepository.Count}] Обновление {repository.Name}");

                Directory.CreateDirectory(versionFolderPath);

                foreach (var asset in release.Assets)
                {
                    if (asset.Name.EndsWith(".zip")) // skip source code zip files
                    {
                        continue;
                    }

                    string filePath = $"{versionFolderPath}/{asset.Name}";

                    if (File.Exists(filePath))
                    {
                        Console.WriteLine($"File {asset.Name} already exists, skipping...");
                        continue;
                    }

                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(asset.BrowserDownloadUrl);
                        response.EnsureSuccessStatusCode();
                        var bytes = await response.Content.ReadAsByteArrayAsync();

                        await File.WriteAllBytesAsync(filePath, bytes);
                    }
                }

                ZipFile.CreateFromDirectory(versionFolderPath, $"{versionFolderPath}.zip");
            }


            plugins.Add(plugin);
            ++processedCount;
        }

        return plugins;
    }
}