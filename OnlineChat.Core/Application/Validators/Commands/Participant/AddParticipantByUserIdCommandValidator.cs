using Application.CQRS.Commands.Participant;
using Application.Entities;
using Application.Interfaces.Repositories;
using Application.Queries;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Resources;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Participant
{
    internal sealed class AddParticipantByUserIdCommandValidator : AbstractValidator<AddParticipantByUserIdCommand>
    {
        private readonly IParticipantRepository _participantRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IIdentityService _identityService;
        private readonly UserManager<User> _userManager;

        public AddParticipantByUserIdCommandValidator(IConversationRepository conversationRepository, IParticipantRepository participantRepository, IIdentityService identityService, UserManager<User> userManager)
        {
            _participantRepository = participantRepository ?? throw new ArgumentNullException(nameof(participantRepository));
            _conversationRepository = conversationRepository ?? throw new ArgumentNullException(nameof(conversationRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

            RuleFor(x => x.UsertId)
                .NotEmpty()
                .MustAsync(async (id, CancellationToken) => await _userManager.FindByIdAsync(id.ToString()) != null).WithMessage(Messages.NotFound);


            RuleFor(x => x)
                .MustAsync(MustNotBePartisipantOfConversation).WithMessage(Messages.AlreadyExists);

            RuleFor(x => x.ConversationId)
                .NotEmpty()
                .MustAsync(_conversationRepository.ExistsAsync).WithMessage(Messages.NotFound)
                .MustAsync(MustBePartisipantOfConversation).WithMessage(Messages.AccessDenied);
        }

        private async Task<bool> MustBePartisipantOfConversation(int conversationId, CancellationToken cancellationToken)
        {
            var currentUserId = _identityService.GetUserId();
            var participant = await _participantRepository.GetByQueryAsync(new ParticipantQuery()
            {
                ConversationId = conversationId,
                UserId = currentUserId
            });

            return participant != null;
        }

        private async Task<bool> MustNotBePartisipantOfConversation(AddParticipantByUserIdCommand command, CancellationToken cancellationToken)
        {
            var participant = await _participantRepository.GetByQueryAsync(new ParticipantQuery()
            {
                ConversationId = command.ConversationId,
                UserId = command.UsertId
            });

            return participant == null;
        }
    }
}
