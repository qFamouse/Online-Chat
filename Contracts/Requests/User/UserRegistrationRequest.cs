﻿namespace Contracts.Requests.User
{
    public class UserRegistrationRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
