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
}