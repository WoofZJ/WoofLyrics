using System.Net.Http.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WoofLyrics.Core;

public class Lyrics(string? original, string? translated, string? romaji)
{
    public readonly string? Original = original;
    public readonly string? Translated = translated;
    public readonly string? Romaji = romaji;
}

public class LyricService
{
    public static readonly string ServerUrl = Environment.GetEnvironmentVariable("LYRICS_SERVER_URL")!;
    public static async Task<Lyrics?> Fetch(string name, string artists, string album, string spotifyId)
    {
        using HttpClient client = new();
        var jsonObj = new
        {
            id = spotifyId,
            name = name,
            album = album,
            artists = artists
        };

        var response = await client.PostAsJsonAsync(ServerUrl, jsonObj);
        if (response.IsSuccessStatusCode)
        {
            string jsons = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<JObject>(jsons)!;
            return new Lyrics(
                json.GetValue("orig")?.ToString(),
                json.GetValue("ts")?.ToString(),
                json.GetValue("roma")?.ToString());
        }
        else
        {
            return null;
        }
    }
}