using AKLC.Domain.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AKLC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public class AdminController
        : ControllerBase
    {
        // =========================================
        // ADMIN DASHBOARD
        // GET: api/Admin/dashboard
        // =========================================

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            return Ok(
                new
                {
                    message =
                        "Welcome to AKLC Admin Dashboard",

                    userName =
                        User.Identity?.Name,

                    role =
                        AppRoles.Admin
                });
        }
    }
}