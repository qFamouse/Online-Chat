﻿using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineChat.Core.Requests.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OnlineChat.Core.SQRS.Commands.User
{
    using OnlineChat.Core.Entities;
    public class SignUpUserCommand : IRequest<User>
    {
        public User User { get; set; }
        public string Password { get; set; }

        public SignUpUserCommand(UserRegistrationRequest request)
        {
            User = new User()
            {
                Name = request.Name,
                UserName = request.Email,
                Email = request.Email
            };

            Password = request.Password;
        }
    }
}