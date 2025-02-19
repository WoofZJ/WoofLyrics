using WoofLyrics.Core;

namespace WoofLyrics.Test;

public class LyricTest
{
    [Fact]
    public void LyricParseTest()
    {
        string lyric = """
            [0,1000]hello(0,100)world(100,200)!(300,500)
            [1000,2000]hello(1000,200)world(1200,300)!(1500,500)
            """;
        LyricParser parser = new(lyric);
        Assert.Equal(2, parser.Lines.Count);
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        foreach (var line in parser.Lines)
        {
            foreach (var word in line.Words)
            {
                Console.WriteLine($"{word.Word}, ({word.StartMs}, {word.EndMs})");
            }
        }
    }

    [Fact]
    public async Task LyricFetchTest()
    {
        string name = "繭色";
        string album = name;
        string artists = "sajou no hana";
        string id = "3z0Rh5nn1cunyc8t3GH7Vs";
        Lyrics? lyrics = await LyricService.Fetch(name, artists, album, id);
        Assert.NotNull(lyrics);
        Assert.NotNull(lyrics?.Original);
        Assert.NotNull(lyrics?.Translated);
        Assert.NotNull(lyrics?.Romaji);
    }
}