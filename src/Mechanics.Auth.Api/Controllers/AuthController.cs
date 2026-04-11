using Mechanics.Auth.Application.Requests;
using Mechanics.Auth.Application.Response;
using Mechanics.Auth.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mechanics.Auth.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AuthController(AuthAppService service) : ControllerBase
{
    /// <summary>
    ///     Gera um token JWT para o usuário fornecido.
    /// </summary>
    /// <returns>Um <see cref="TokenResponse"/> contendo o token JWT e refresh token.</returns>
    /// <response code="200">Usuário autenticado com sucesso.</response>
    /// <response code="401">Usuário ou senha inválidos.</response>
    [HttpPost]
    [Consumes(typeof(LoginRequest), "application/json")]
    [Produces("application/json", Type = typeof(TokenResponse))]
    [ProducesResponseType(typeof(TokenResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await service.Login(request);
        return result is not null ? Ok(result) : Unauthorized();
    }

    /// <summary>
    ///     Gera um token JWT a partir de um refresh token.
    /// </summary>
    /// <returns>Um <see cref="TokenResponse"/> contendo o token JWT e um refresh token estendido.</returns>
    /// <response code="200">Usuário autenticado com sucesso.</response>
    /// <response code="401">Token inválido.</response>
    [HttpPost]
    [Consumes(typeof(RefreshTokenRequest), "application/json")]
    [Produces("application/json", Type = typeof(TokenResponse))]
    [ProducesResponseType(typeof(TokenResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var result = await service.Refresh(request);
        return result is not null ? Ok(result) : Unauthorized();
    }
}
