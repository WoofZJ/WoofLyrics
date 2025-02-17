using WoofLyrics.Core;

namespace WoofLyrics.Test;

public class SpotifyAuthTest
{
    // [Fact]
    // public async Task AuthenticationFailTest()
    // {
    //     if (File.Exists(SpotifyAuth.CredentialsPath))
    //     {
    //         File.Delete(SpotifyAuth.CredentialsPath);
    //     }
    //     bool authSuccess = await SpotifyAuth.StartAuthenticationAsync();
    //     Assert.False(authSuccess);
    //     Assert.Equal(SpotifyAuth.State.Aborted, SpotifyAuth.CurrentState);
    // }

    [Fact]
    public async Task AuthenticationSuccessTest()
    {
        if (File.Exists(SpotifyAuth.CredentialsPath))
        {
            File.Delete(SpotifyAuth.CredentialsPath);
        }
        bool authSuccess = await SpotifyAuth.StartAuthenticationAsync();
        Assert.True(authSuccess);
        Assert.Equal(SpotifyAuth.State.Authenticated, SpotifyAuth.CurrentState);
    }

    [Fact]
    public async Task AuthorizationTest()
    {
        var config = await SpotifyAuth.StartAuthorizationAsync();
        Assert.NotNull(config);
    }
}
