using System.Text.Json.Serialization;

namespace Gml.Plugins.Server.Models;

public class GmlPlugin
{
    public required string Name { get; set; }
    public List<GmlVersion> Versions { get; set; } = [];
    public required string Author { get; set; }
    public string? Description { get; set; }
    public required string Homepage { get; set; }
    public string? License { get; set; }
    public int Stars { get; set; }
}

public class GmlVersion
{
    public string Name { get; set; }
    public string Body { get; set; }
    public string Version { get; set; }
    [JsonIgnore] public string Folder { get; set; }
}