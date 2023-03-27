using Microsoft.AspNetCore.Components;
using MudBlazor;
using SmartVillages.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmartVillages.Shared.UserModels;

namespace SmartVillages.Client.Pages.UserSign
{
    public class SignUpBase : ComponentBase
    {
        [Parameter] public bool IsLeftOpened { get; set; }
        [Parameter] public EventCallback GoBack { get; set; }
        [Parameter] public EventCallback OpenSignIn { get; set; }
        
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public HttpClient Http { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }

        public User UserModel { get; set; } = new User();
        public string Message { get; set; } = "";
        public List<Place> Places { get; set; } = new List<Place>();

        protected override async Task OnInitializedAsync()
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;
            Snackbar.Configuration.SnackbarVariant = Variant.Filled;
            UserModel.Place = new Place { Id = 0, Name = "", PostalCode = "00000" };
            await GetPlaces();
        }

        public async Task HandleValidSubmit()
        {
            Snackbar.Clear();
            Snackbar.Add("Validateing..", Severity.Success);

            await CreateUser();
        }

        public void HandleInvalidSubmit()
        {
            Snackbar.Clear();
            Snackbar.Add("All fields are required!", Severity.Error);
            if(!UserModel.TermsAndConditions)
                Snackbar.Add("You must agree to terms and conditions!", Severity.Error);
        }

        public async Task GetPlaces()
        {
            Places = new List<Place>();
            var response = await Http.GetAsync("api/Places");
            if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                Places = await response.Content.ReadFromJsonAsync<List<Place>>();
            }
        }

        public async Task CreateUser()
        {
            int user_type = IsLeftOpened ? 2 : 1;
            
            try
            {
                var response = await Http.PostAsJsonAsync($"api/users/postuser/{user_type}", UserModel);

                Snackbar.Clear();
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    Snackbar.Add("Već postoji korisnik s tim email-om ili oib-om.", Severity.Error);
                }
                else
                {
                    Snackbar.Add("<p class='text-center'>SUCCESSFULLY REGISTERED! </br> Please go check you email for further validation.</p>", Severity.Success);
                    User returnValue = await response.Content.ReadFromJsonAsync<User>();
                    await SendEmail(returnValue);
                    await GoBack.InvokeAsync();
                }
            }
            catch (HttpRequestException ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
                throw;
            }
        }

        // Password
        public bool isPassVisiable;
        public InputType PasswordInput = InputType.Password;
        public string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
        public void ShowHidePass()
        {
            if(isPassVisiable)
            {
                isPassVisiable = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }
            else
            {
                isPassVisiable = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
        }

        public async Task SendEmail(User returnValue)
        {
            try
            {
                var response = await Http.PostAsJsonAsync($"api/users/SendEmail", returnValue);

                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    Snackbar.Add("Greška kod slanja emaila.", Severity.Error);
                }
                else
                {
                    Snackbar.Add("Email uspješno poslan.", Severity.Success);
                }
            }
            catch (HttpRequestException ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }

    }
}
