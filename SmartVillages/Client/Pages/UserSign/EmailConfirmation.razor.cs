using Microsoft.AspNetCore.Components;
using MudBlazor;
using SmartVillages.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SmartVillages.Client.Pages.UserSign
{
    public class EmailConfirmationBase : ComponentBase
    {
        [Parameter] public string Code { get; set; }
        [Parameter] public string Oib { get; set; }

        [Inject] public HttpClient Http { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }

        public bool IsValid { get; set; }
        public string Message { get; set; }

        protected async override Task OnParametersSetAsync()
        {
            // provjeriti jel vec validirano i preusmjeriti..
            
            try
            {
                var response = await Http.PostAsJsonAsync($"api/users/confirmemail/{Oib}", Code);

                Snackbar.Clear();
                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Snackbar.Add("Email uspješno validiran!", Severity.Success);
                    Message = "Poslali smo vam na mail vaš SecretCode s kojim se možete prijaviti.";
                    IsValid = true;
                    StateHasChanged();
                }
            }
            catch (HttpRequestException ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
                throw;
            }
            
            // kreirati shared service EmailSender i prebaciti tako na api sve..

        }
    }
}
