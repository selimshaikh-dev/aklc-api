using System;
using System.Collections.Generic;
using System.Text;

namespace AKLC.Application.DTOs.Auth
{
    public class LoginRequest
    {
        public string MobileNumber { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}