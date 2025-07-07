using AuthBlocksAPI.Models;

namespace AuthBlocksAPI.Common;

public static class JwtSettingsTools
{
    public static JwtSettings LoadFromFile(string filePathFromBase)
    {
        using var reader = new StreamReader(File.OpenRead(filePathFromBase));
        
        string json = reader.ReadToEnd();
        var endpoints = System.Text.Json.JsonSerializer.Deserialize<JwtSettings>(json);
        reader.Close();
        return endpoints ?? throw new Exception("Could not read JWT Settings");
    }

    public static void SaveToFile(string filePathFromBase, JwtSettings endpoints)
    {
        using var writer = new StreamWriter(File.Create(filePathFromBase));
        
        string json = System.Text.Json.JsonSerializer.Serialize(endpoints);
        writer.WriteLine(json);
        writer.Close();
    }
}