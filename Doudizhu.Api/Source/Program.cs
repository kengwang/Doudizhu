using AsyncAwaitBestPractices;
using Doudizhu.Api.Extensions.DependencyInjection;
using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.GameService;
using Doudizhu.Api.Service.Hubs;

using Doudizhu.Common;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthenticationCookie(TimeSpan.FromDays(3));
builder.Services.AddAuthorization();
builder.Services.AddFastEndpoints(o => o.SourceGeneratorDiscoveredTypes.AddRange(DiscoveredTypes.All))
       .SwaggerDocument();
builder.Services.AddSignalR();
builder.Services.AddSingleton<GameContainer>();
builder.Services.AddDependencyInjectionMarkerFrom<Program>();
builder.Services.AddDependencyInjectionMarkerFrom<Marker>();
builder.Services.AddDependencyInjectionImplements<CardPattern, CardPattern>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policyBuilder  =>
        {
            policyBuilder.SetIsOriginAllowed(_=>true)
                         .AllowAnyHeader()
                         .AllowAnyMethod()
                         .AllowCredentials();
        });
});
var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(c =>
{
    c.Endpoints.ShortNames = true;
    c.Endpoints.RoutePrefix = "api";
    c.Errors.UseProblemDetails();
});
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");
    app.UseSwaggerGen();
    app.MapScalarApiReference();
}

SafeFireAndForgetExtensions.Initialize(false);
app.UseSwaggerGen();
app.MapHub<GameHub>("/hub/game");
app.Run("http://*:44460");