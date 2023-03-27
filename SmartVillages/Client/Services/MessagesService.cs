using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using SmartVillages.Client.Pages;
using SmartVillages.Shared.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SmartVillages.Client.Services
{
    public class MessagesService
    {
        [Inject] public HttpClient Http { get; set; }
        public UserConnection UserConnection { get; set; } = new UserConnection();
        public HubConnection Connection { get; private set; }

        public MessagesService(HttpClient client)
        {
            Http = client;
        }

        public async Task ConnectToServer(User user, MessagesUpdateService messagesUpdateService, ISnackbar Snackbar)
        {
            var connectionHubUrl = "";
            connectionHubUrl = $"{Http.BaseAddress}connectionhub";

            Connection = new HubConnectionBuilder().WithUrl(connectionHubUrl).Build();

            await Connection.StartAsync();

            Connection.Closed += async (s) =>
            {
                Console.WriteLine("Connection closed");
            };

            Connection.On("new_message", () => SendNotificationToUser(messagesUpdateService, Snackbar));

            UserConnection userConnection = new UserConnection { Id = 0, ConnectionId = Connection.ConnectionId, IsActive = true, UserId = user.Id.ToString() };
            var response = await Http.PostAsJsonAsync($"api/userconnections/postuserconnection", userConnection);

            UserConnection = await response.Content.ReadFromJsonAsync<UserConnection>();
        }

        public async Task SendNotificationToUser(MessagesUpdateService messagesUpdateService, ISnackbar Snackbar)
        {
            await messagesUpdateService.SendNotificationToUser(Snackbar);
        }

        // DOKUMENTACIJA hehe
        // ++ Kad se korisnik prijavi pokrene se ovaj ConnectToServer() dobije connectionid i spremi ga u bazu, zatim kad netko pozove SendMessageToUser() u MessagesController taj korisnik dobije obavijest

        // Jos za napraviti..

        // ++ Dohvatiti aktivne korisnike i dati im zeleni kruzic u chatu
        // ++ Na odjavi staviti IsActive u false
        // ++ I prilikom svake prijave da se kreira novi zapis.. ako je korisnik vec ostao ulogiran jel 

        // Kad se posalje poruka poslati i obavijest pozivom one navedene gore i azurirat chat, za sto je portrebno prvo dohvatiti korisnikov connectionid preko userid-a

        // Kad se kkorisnik odjavi salje se isto "obavjest" pa mu kod ikonice prelazi iz zelenog u crveno

        // NIŠTA OD TOGA DA PRIKAZUJE TKO JE AKTIVAN A TKO NE
    }
}
