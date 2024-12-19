using Microsoft.AspNetCore.Http.HttpResults;

namespace Doudizhu.Api.Endpoints.Game;

public class GetGameEndpoint : EndpointWithoutRequest<Results<Ok<GetGameResponse>, NotFound>>
{
    
}

public class GetGameResponse
{
    
}