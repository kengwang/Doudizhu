using System.Text.Json.Serialization;
using Doudizhu.Api.Models;

namespace Doudizhu.Api.Endpoints.User;

public class UserLoginEndpoint : Endpoint<UserLoginRequest, UserLoginResponse>
{

    public UserLoginEndpoint()
    {
        Post("/user/login");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(UserLoginRequest req, CancellationToken ct)
    {
        var token = JwtBearer.CreateToken(
            option =>
            {
                option.SigningKey = Constants.JwtSigningKey;
                option.User["name"] = req.Name;
            });
        await SendAsync(new() {Token = token}, cancellation: ct);
    }
}

public class UserLoginRequest
{
    [JsonPropertyName("name")] public string Name { get; set; }
}

public class UserLoginResponse
{
    [JsonPropertyName("token")] public string Token { get; set; }
}