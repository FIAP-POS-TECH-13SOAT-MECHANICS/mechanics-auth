using Mechanics.Auth.Application.Requests;
using Mechanics.Auth.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mechanics.Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController(AuthAppService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await service.Login(request);
        return result is not null ? Ok(result) : Unauthorized();
    }

    [HttpPost]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var result = await service.Refresh(request);
        return result is not null ? Ok(result) : Unauthorized();
    }
}
