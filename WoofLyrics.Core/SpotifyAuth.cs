using Newtonsoft.Json;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using static SpotifyAPI.Web.Scopes;

namespace WoofLyrics.Core;
public class SpotifyAuth
{
    public const string CredentialsPath = "credentials.json";
    private static readonly string? clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
    private static readonly EmbedIOAuthServer _server = new(new Uri("http://localhost:5543/callback"), 5543);

    public enum State
    {
        Idle,
        Authenticating,
        Authenticated,
        Aborted,
        Timeout,
    };

    private static State _state = State.Idle;

    public static State CurrentState
    {
        get => _state;
    }
    public static bool NeedsAuthentication() => !File.Exists(CredentialsPath);

    public static async Task<bool> StartAuthenticationAsync()
    {
        if (!NeedsAuthentication())
        {
            return true;
        }

        _state = State.Authenticating;
        var (verifier, challenge) = PKCEUtil.GenerateCodes();
        await _server.Start();

        _server.AuthorizationCodeReceived += async (sender, response) =>
        {
            await _server.Stop();
            var token = await new OAuthClient().RequestToken(
                new PKCETokenRequest(clientId!, response.Code, _server.BaseUri, verifier)
            );

            await File.WriteAllTextAsync(CredentialsPath, JsonConvert.SerializeObject(token));
            _state = State.Authenticated;
        };

        _server.ErrorReceived += (sender, error, response) =>
        {
            _state = State.Aborted;
            return Task.CompletedTask;
        };

        var request = new LoginRequest(_server.BaseUri, clientId!, LoginRequest.ResponseType.Code)
        {
            CodeChallenge = challenge,
            CodeChallengeMethod = "S256",
            Scope =
            [
                UserReadCurrentlyPlaying,
            ]
        };

        var uri = request.ToUri();
        try
        {
            BrowserUtil.Open(uri);
            int count = 0;
            while (_state == State.Authenticating)
            {
                if (count > 60)
                {
                    _server.Dispose();
                    _state = State.Timeout;
                    return false;
                }
                await Task.Delay(1000);
                count += 1;
            }
            _server.Dispose();
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    public static async Task<SpotifyClientConfig?> StartAuthorizationAsync()
    {
        if (NeedsAuthentication())
        {
            return null;
        }
        var json = await File.ReadAllTextAsync(CredentialsPath);
        var token = JsonConvert.DeserializeObject<PKCETokenResponse>(json);

        var authenticator = new PKCEAuthenticator(clientId!, token!);
        authenticator.TokenRefreshed += (sender, token) => File.WriteAllText(CredentialsPath, JsonConvert.SerializeObject(token));

        var config = SpotifyClientConfig.CreateDefault()
            .WithAuthenticator(authenticator);

        return config;
    }
}