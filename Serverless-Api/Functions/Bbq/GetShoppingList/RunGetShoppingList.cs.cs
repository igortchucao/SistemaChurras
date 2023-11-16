using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.Repositories;

namespace Serverless_Api
{
    public partial class RunGetShoppingList
    {
        private readonly IBbqRepository _bbqStore;
        public RunGetShoppingList(IBbqRepository bbqStore)
        {
            _bbqStore = bbqStore;
        }

        [Function(nameof(RunGetShoppingList))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras/{id}/ListaCompras")] HttpRequestData req, string id)
        {
            var bbq = await _bbqStore.GetAsync(id);

            var qntConfirmados = bbq.InvitationsAccepted.Count();

            var qntCarneBoi     = "Carne de Boi: "   + (150  * qntConfirmados)       + "g" +
                "\n (Pode ser Picanha ou Contra-fil� se a Trinca for pagar. Se n�o, ponta do peito j� serve)\n";
            
            var qntCarnePorco   = "Carne de Porco: " + (100  * qntConfirmados)       + "g" +
                "\n (Cabe�a de lombo)\n";

            var qntLinguica     = "Lingui�a: "       + (50   * qntConfirmados)       + "g" +
                "\n (Perdig�o ou alguma artesanal)\n";

            var qntArroz        = "Arroz: "          + (50   * qntConfirmados)       + "g" +
                "\n (Algum premium que n�o precise lavar)\n";

            var qntFarofa       = "Farofa: "         + (50   * qntConfirmados)       + "g" +
                "\n (Farinha de mandioca, alho e manteiga)\n";

            var qntRefri        = "Refrigerante: "   + (500  * qntConfirmados)/2000  + " garrafas pets";
            var qntCerveja      = "Cerveja: "        + (3    * qntConfirmados)       + " latinhas";

            return await req.CreateResponse(HttpStatusCode.Created, 
                qntCarneBoi +
                qntCarnePorco +
                qntLinguica +
                qntArroz +
                qntFarofa +
                qntRefri +
                qntCerveja
            );
        }
    }
}
