using Doudizhu.Api.Extensions.DependencyInjection;
using Doudizhu.Api.Models;
using Doudizhu.Api.Service.Hubs;
using Doudizhu.Api.Service.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthenticationCookie(TimeSpan.FromDays(3));
builder.Services.AddAuthorization();
builder.Services.AddFastEndpoints(o => o.SourceGeneratorDiscoveredTypes.AddRange(DiscoveredTypes.All));
builder.Services.SwaggerDocument();
builder.Services.AddSignalR();
builder.Services.AddDbContextPool<ApplicationDbContext>(
    option =>
    {
        option.UseInMemoryDatabase("database");
    });
builder.Services.Configure<JwtCreationOptions>(
    opt =>
    {
        opt.SigningKey = Constants.JwtSigningKey;
    });
builder.Services.AddDependencyInjectionMarkerFrom<Program>();
var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
    c.Errors.UseProblemDetails();
});
app.UseSwaggerGen();
app.MapHub<GameHub>("/hub/game");
app.Run();

public partial class Program { }