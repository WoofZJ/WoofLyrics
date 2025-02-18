using System.Text.RegularExpressions;

namespace WoofLyrics.Core;

public class LyricWord(int startMs, int duraMs, string word)
{
    public readonly int StartMs = startMs;
    public readonly int EndMs = startMs + duraMs;
    public string Word = word;
}

public partial class LyricLine
{
    public readonly int StartMs;
    public readonly int EndMs;
    public List<LyricWord> Words = [];
    private static readonly Regex _regex = WordRegex();
    public LyricLine(int startMs, int duraMs, string line)
    {
        StartMs = startMs;
        EndMs = startMs + duraMs;
        var matches = _regex.Matches(line);
        foreach (Match match in matches)
        {
            if (match.Groups.Count == 4)
            {
                Words.Add(
                    new LyricWord(
                        int.Parse(match.Groups[2].Value),
                        int.Parse(match.Groups[3].Value),
                        match.Groups[1].Value
                    )
                );
            }
        }
    }

    [GeneratedRegex(@"(.*?)\((\d+),(\d+)\)")]
    private static partial Regex WordRegex();
}

public partial class LyricParser
{
    public List<LyricLine> Lines = [];
    private static readonly Regex _regex = LineRegex();
    public LyricParser(string lyric)
    {
        var matches = _regex.Matches(lyric);
        foreach (Match match in matches)
        {
            if (match.Groups.Count == 4)
            {
                Lines.Add(
                    new LyricLine(
                        int.Parse(match.Groups[1].Value),
                        int.Parse(match.Groups[2].Value),
                        match.Groups[3].Value
                    )
                );
            }
        }
    }

    [GeneratedRegex(@"\[(\d+),(\d+)\](.+)")]
    private static partial Regex LineRegex();
}