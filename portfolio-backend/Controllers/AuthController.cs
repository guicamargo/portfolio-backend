using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GoogleAuthSettings _googleAuthSettings;

    public AuthController(IHttpClientFactory httpClientFactory, IOptions<GoogleAuthSettings> googleAuthSettings)
    {
        _httpClientFactory = httpClientFactory;
        _googleAuthSettings = googleAuthSettings.Value;
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthRequest request)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "client_id", _googleAuthSettings.ClientId },
            { "client_secret", _googleAuthSettings.ClientSecret },
            { "code", request.Code },
            { "grant_type", "authorization_code" },
            { "redirect_uri", "http://localhost:5173" } // URL do frontend
        }));

        if (!response.IsSuccessStatusCode)
            return BadRequest("Erro ao autenticar com o Google");

        var tokenData = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();
        return Ok(new { token = tokenData.IdToken, refreshToken = tokenData.RefreshToken });
    }
}

public class GoogleAuthRequest
{
    public string Code { get; set; }
}

public class GoogleTokenResponse
{
    public string IdToken { get; set; }
    public string RefreshToken { get; set; }
}
