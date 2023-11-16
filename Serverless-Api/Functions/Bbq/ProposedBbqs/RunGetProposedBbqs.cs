using System.Net;
using CrossCutting;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Serverless_Api
{
    public partial class RunGetProposedBbqs
    {
        private readonly Person _user;
        private readonly IBbqRepository _bbqs;
        private readonly IInviteRepository _invites;
        private readonly IPersonRepository _invitesPerson;

        public RunGetProposedBbqs(IBbqRepository bbqs, IPersonRepository personR, IInviteRepository invites, Person user)
        {
            _user = user;
            _bbqs = bbqs;
            _invites = invites;
            _invitesPerson = personR;
        }

        [Function(nameof(RunGetProposedBbqs))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras")] HttpRequestData req)
        {
            var snapshots = new List<object>();

            var person = await _invitesPerson.GetAsync(_user.Id);

            if (person == null)
            {
                return req.CreateResponse(HttpStatusCode.NoContent);
            }

            foreach (var inviteId in person.Invites)
            {
                var invite = await _invites.GetAsync(inviteId);
                var bbq = await _bbqs.GetAsync(invite.BbqId);
                snapshots.Add(bbq.TakeSnapshot());
            }

            return await req.CreateResponse(HttpStatusCode.Created, snapshots);
        }
    }
}
