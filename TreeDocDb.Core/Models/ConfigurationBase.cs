using System.IO;

namespace TreeDocDb.Core.Models;

using Newtonsoft.Json;

[JsonObject]
public class ConfigurationBase
{
    [JsonIgnore] public string SrcFile { get; private set; } = string.Empty;

    public static ConfigurationBase Open(string srcFile)
    {
        var json = File.ReadAllText(srcFile);
        
        var config = JsonConvert.DeserializeObject<ConfigurationBase>(json) ??
                     throw new InvalidDataException("Invalid configuration file");

        config.SrcFile = srcFile;

        return config;
    }

    public void Save(string? file = null)
    {
        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(file ?? SrcFile, json);
    }
}