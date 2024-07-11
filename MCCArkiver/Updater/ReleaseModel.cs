using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MCCArkiver.Updater;

public class ReleaseModel
{
    [JsonPropertyName("author")]
    public Author Author { get; }
    [JsonPropertyName("tag_name")]
    public string Tag { get; set; }
    [JsonPropertyName("body")]
    public string Description { get; set; }
    [JsonPropertyName("assets")]
    public List<GitAsset> Assets { get; set; }
}

public class Author
{
    [JsonPropertyName("avatar_url")]
    public string Avatar { get; set; }

    public string Avurl()
    {
        return $"{Avatar}.png";
    }
}

public class GitAsset
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("browser_download_url")]
    public string Download {get; set; }
    [JsonPropertyName("size")]
    public int SizeRaw { get; set; }

    public string Size()
    {
        return $"Size: {SizeRaw/1024/1024} MB";
    }
}