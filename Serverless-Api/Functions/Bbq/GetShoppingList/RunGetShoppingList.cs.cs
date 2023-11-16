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
                "\n (Pode ser Picanha ou Contra-filé se a Trinca for pagar. Se não, ponta do peito já serve)\n";
            
            var qntCarnePorco   = "Carne de Porco: " + (100  * qntConfirmados)       + "g" +
                "\n (Cabeça de lombo)\n";

            var qntLinguica     = "Linguiça: "       + (50   * qntConfirmados)       + "g" +
                "\n (Perdigão ou alguma artesanal)\n";

            var qntArroz        = "Arroz: "          + (50   * qntConfirmados)       + "g" +
                "\n (Algum premium que não precise lavar)\n";

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
