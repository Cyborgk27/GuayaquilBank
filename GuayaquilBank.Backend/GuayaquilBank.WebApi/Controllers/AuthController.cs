using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Dtos.Authentication.Request;
using Microsoft.AspNetCore.Mvc;

namespace GuayaquilBank.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationAppService _appService;

        public AuthController(IAuthenticationAppService appService)
        {
            _appService = appService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _appService.LoginAsync(request);
            return Ok(response);
        }

    }
}
