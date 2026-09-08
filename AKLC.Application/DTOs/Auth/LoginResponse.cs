using System;
using System.Collections.Generic;
using System.Text;

namespace AKLC.Application.DTOs.Auth
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public UserInfoDto User { get; set; } = new();
    }

    public class UserInfoDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}