using Domain.Events;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using Grpc.Core;
using Domain.Entities.Enums;

namespace Serverless_Api
{
    public partial class RunAcceptInvite
    {
        private readonly Person _user;
        private readonly IInviteRepository _repository;
        private readonly IBbqRepository _repositoryBBQ;
        public RunAcceptInvite(IInviteRepository repository, IBbqRepository bbqRepository, Person user)
        {
            _repository = repository;
            _repositoryBBQ = bbqRepository;
            _user = user;
        }

        [Function(nameof(RunAcceptInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/accept")] HttpRequestData req, string inviteId)
        {
            var invite = await _repository.GetAsync(inviteId);

            if (invite == null)
            {
                return req.CreateResponse(HttpStatusCode.NoContent);
            }

            if(invite.PersonId != _user.Id) 
            { 
                return req.CreateResponse(HttpStatusCode.Unauthorized);
            }

            if(invite.Status != InviteStatus.Accepted)
            {
                var inviteWasAccepted = new InviteWasAccepted { InviteId = inviteId };
                invite.Apply(inviteWasAccepted);
                await _repository.SaveAsync(invite);

                var bbq = await _repositoryBBQ.GetAsync(invite.BbqId);

                if (!bbq.InvitationsAccepted.Contains(inviteId))
                {
                    bbq.Apply(inviteWasAccepted);
                    _repositoryBBQ.SaveAsync(bbq);
                }
            }

            return await req.CreateResponse(HttpStatusCode.OK, invite.TakeSnapshot());
        }
    }
}
