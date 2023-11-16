using Domain;
using Eveneum;
using CrossCutting;
using Domain.Events;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using static Domain.ServiceCollectionExtensions;
using System.Net;
using Domain.Entities.Enums;

namespace Serverless_Api
{
    public partial class RunDeclineInvite
    {
        private readonly Person _user;
        private readonly IInviteRepository _repository;
        private readonly IBbqRepository _repositoryBBQ;

        public RunDeclineInvite(IInviteRepository repository, IBbqRepository bbqRepository, Person person)
        {
            _repository = repository;
            _repositoryBBQ = bbqRepository;
            _user = person;
        }

        [Function(nameof(RunDeclineInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/decline")] HttpRequestData req, string inviteId)
        {
            var invite = await _repository.GetAsync(inviteId);

            if (invite == null)
            {
                return req.CreateResponse(HttpStatusCode.NoContent);
            }

            if (invite.PersonId != _user.Id)
            {
                return req.CreateResponse(HttpStatusCode.Unauthorized);
            }

            if (invite.Status != InviteStatus.Declined)
            {
                var inviteWasDeclined = new InviteWasDeclined { InviteId = inviteId };
                invite.Apply(inviteWasDeclined);
                await _repository.SaveAsync(invite);

                var bbq = await _repositoryBBQ.GetAsync(invite.BbqId);

                if (bbq.InvitationsAccepted.Contains(inviteId))
                {
                    bbq.Apply(inviteWasDeclined);
                    _repositoryBBQ.SaveAsync(bbq);
                }
            }

            return await req.CreateResponse(HttpStatusCode.OK, invite.TakeSnapshot());
        }
    }
}
