using Doudizhu.Api.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = Constants.JwtSigningKey);
builder.Services.AddAuthorization();
builder.Services.AddFastEndpoints(o => o.SourceGeneratorDiscoveredTypes.AddRange(DiscoveredTypes.All));
builder.Services.SwaggerDocument();
builder.Services.AddSignalR();
var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(c => c.Errors.UseProblemDetails());
app.UseSwaggerGen();

app.Run();

public partial class Program { }