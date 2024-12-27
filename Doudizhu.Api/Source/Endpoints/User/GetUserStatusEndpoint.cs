using System.Security.Claims;
using Doudizhu.Api.Service.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Doudizhu.Api.Endpoints.User;

public class GetUserStatusEndpoint(ApplicationDbContext dbContext) : Ep.NoReq.Res<Results<Ok<Models.User>, NotFound>>
{
    public override void Configure()
    {
        Get("/user/status");
    }

    public override async Task<Results<Ok<Models.User>, NotFound>> ExecuteAsync(CancellationToken ct)
    {
        var userIdStr = User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userIdStr == null)
        {
            return TypedResults.NotFound();
        }
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return TypedResults.NotFound();
        }
        var user = await dbContext.Users.FindAsync([userId], ct);
        
        if (user == null)
        {
            return TypedResults.NotFound();
        }
        
        return TypedResults.Ok(user);
    }
}