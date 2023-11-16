using Eveneum;
using System.Net;
using CrossCutting;
using Domain.Events;
using Domain.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.Repositories;

namespace Serverless_Api
{
    public partial class RunShowBbqCreated
    {
        private readonly IBbqRepository _bbqsStore;

        public RunShowBbqCreated(IBbqRepository eventStore)
        {
            _bbqsStore = eventStore;
        }

        [Function(nameof(RunShowBbqCreated))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras/{id}/moderar")] HttpRequestData req, string id)
        {
            var bbq = await _bbqsStore.GetAsync(id);

            if (bbq == null)
            {
                return req.CreateResponse(HttpStatusCode.NoContent);
            }

            return await req.CreateResponse(HttpStatusCode.OK, bbq.TakeSnapshot());
        }
    }
}
