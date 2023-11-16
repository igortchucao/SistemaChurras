using Eveneum;
using System.Net;
using CrossCutting;
using Domain.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain;

namespace Serverless_Api
{
    public partial class RunRebuildBbq
    {
        private readonly Person _user;
        private readonly SnapshotStore _snapshots;
        private readonly IEventStore<Bbq> _bbqsStore;
        private readonly IEventStore<Person> _peopleStore;
        public RunRebuildBbq(IEventStore<Bbq> eventStore, IEventStore<Person> peopleStore, SnapshotStore snapshots, Person user)
        {
            _user = user;
            _snapshots = snapshots;
            _bbqsStore = eventStore;
            _peopleStore = peopleStore;
        }

        [Function(nameof(RunRebuildBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras/{id}/reconstroi")] HttpRequestData req, string id)
        {
            var stream = await _bbqsStore.ReadStream(id, new ReadStreamOptions { IgnoreSnapshots = true, FromVersion = 0, MaxItemCount = 100 });

            var events = stream.Stream?.Events;

            var churras = new Bbq();

            var loadedEvents = events?.Select(@event => (IEvent)@event.Body);

            churras.Rehydrate(loadedEvents);

            return await req.CreateResponse(HttpStatusCode.Created, churras.TakeSnapshot());

        }
    }
}
