﻿using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OnlineChat.Core.Commands.Users;
using OnlineChat.Core.Configurations;
using OnlineChat.Core.Entities;
using OnlineChat.Core.Exceptions;
using OnlineChat.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OnlineChat.Core.CommandHandlers.Users
{
    public class RegistrationCommandHandler : IRequestHandler<RegistrationCommand, User>
    {
        private readonly UserManager<User> _userManager;
        private readonly IdentityConfiguration _identityConfiguration;

        public RegistrationCommandHandler(
            UserManager<User> userManager, 
            IOptions<IdentityConfiguration> identityConfiguration)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _identityConfiguration = identityConfiguration.Value ?? throw new ArgumentNullException(nameof(identityConfiguration));
        }

        public async Task<User> Handle(RegistrationCommand request, CancellationToken cancellationToken)
        {

            IdentityResult result = await _userManager.CreateAsync(request.User, request.Password);

            if (!result.Succeeded)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, result.Errors.First().Description);
            }

            result = await _userManager.AddToRoleAsync(request.User, _identityConfiguration.DefaultRole);

            if (!result.Succeeded)
            {
                throw new HttpStatusCodeException(HttpStatusCode.InternalServerError, result.Errors.First().Description);
            }

            return request.User;
        }
    }
}
