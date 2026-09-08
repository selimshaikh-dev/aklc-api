using AKLC.Application.DTOs.Auth;
using AKLC.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AKLC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(
            IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.MobileNumber) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    message =
                        "Mobile number and password are required."
                });
            }

            var result =
                await _authService.LoginAsync(
                    request,
                    cancellationToken);

            if (result is null)
            {
                return Unauthorized(new
                {
                    message =
                        "Invalid mobile number or password."
                });
            }

            return Ok(result);
        }
    }
}