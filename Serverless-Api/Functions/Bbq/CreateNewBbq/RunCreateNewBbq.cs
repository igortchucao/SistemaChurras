using Eveneum;
using System.Net;
using CrossCutting;
using Domain.Events;
using Domain.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Serverless_Api.Middlewares;

namespace Serverless_Api
{
    public partial class RunCreateNewBbq
    {
        private readonly Person _user;
        private readonly SnapshotStore _snapshots;
        private readonly IPersonRepository _peopleStore;
        private readonly IEventStore<Bbq> _bbqsStore;
        private readonly IEventStore<Invite> _inviteStore;

        public RunCreateNewBbq(IEventStore<Bbq> eventStore, IPersonRepository peopleStore, IEventStore<Invite> inviteStore, SnapshotStore snapshots, Person user)
        {
            _user = user;
            _snapshots = snapshots;
            _bbqsStore = eventStore;
            _peopleStore = peopleStore;
            _inviteStore = inviteStore;
        }

        [Function(nameof(RunCreateNewBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "churras")] HttpRequestData req)
        {
            var input = await req.Body<NewBbqRequest>();

            if (input == null)
            {
                return await req.CreateResponse(HttpStatusCode.BadRequest, "input is required.");
            }

            var churras = new Bbq();
            churras.Apply(new BbqCreated(Guid.NewGuid(), input.Date, input.Reason, input.IsTrincasPaying));

            await _bbqsStore.WriteToStream(
                churras.Id, 
                churras.Changes.Select(evento => 
                    new EventData(churras.Id, 
                        evento, new { CreatedBy = _user.Id }, 
                        churras.Version, 
                        DateTime.Now.ToString()
                        )
                    ).ToArray(), 
                expectedVersion: churras.Version == 0 ? null : churras.Version);

            var churrasSnapshot = churras.TakeSnapshot();

            var Lookups = await _snapshots.AsQueryable<Lookups>("Lookups").SingleOrDefaultAsync();


            foreach (var personId in Lookups.ModeratorIds.Concat(Lookups.PeopleIds))
            {
                var invite = new Invite();

                var @event = new InviteSend(Guid.NewGuid().ToString(), churras.Date, DateTime.Now, churras.Reason, personId, churras.Id);
                invite.Apply(@event);

                var person = await _peopleStore.GetAsync(personId);
                person.Apply(new PersonHasBeenInvitedToBbq(invite.Id));

                _peopleStore.SaveAsync(person);

                await _inviteStore.WriteToStream(
                invite.Id,
                invite.Changes.Select(evento =>
                    new EventData(invite.Id,
                        evento, new { CreatedBy = _user.Id },
                        invite.Version,
                        DateTime.Now.ToString()
                        )
                    ).ToArray(),
                expectedVersion: invite.Version == 0 ? null : invite.Version);
            }


            return await req.CreateResponse(HttpStatusCode.Created, churrasSnapshot);
        }
    }
}
