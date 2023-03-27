using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartVillages.Client.Shared;
using SmartVillages.Shared;
using MudBlazor;
using System.Net.Http;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using SmartVillages.Shared.UserModels;
using Microsoft.AspNetCore.SignalR.Client;
using SmartVillages.Client.Services;

namespace SmartVillages.Client.Pages.UserSign
{
    public class SignInBase : ComponentBase
    {
        [Parameter] public bool IsLeftOpened { get; set; }
        [Parameter] public EventCallback GoBack { get; set; }
        [Parameter] public EventCallback OpenSignUp { get; set; }

        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public MessagesUpdateService MessagesUpdateService { get; set; }
        [Inject] public HttpClient Http { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] ILocalStorageService LocalStorage { get; set; }

        [Inject] public MessagesService MessagesService { get; set; }

        public UserSignIn UserModel { get; set; } = new UserSignIn();
        public User User { get; set; } = new User();

        /* neka animacija dok se logira */
        public bool Loading { get; set; }

        public async Task Previouse()
        {
            await GoBack.InvokeAsync();
        }

        public async Task HandleValidSubmit()
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;
            Snackbar.Configuration.SnackbarVariant = Variant.Filled;
            Snackbar.Add("Validateing..", Severity.Success);

            await Login();
        }

        public void HandleInvalidSubmit()
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;
            Snackbar.Configuration.SnackbarVariant = Variant.Filled;
            Snackbar.Add("All fields are required!", Severity.Error);
        }

        // Password
        public bool isPassVisiable;
        public InputType PasswordInput = InputType.Password;
        public string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
        public void ShowHidePass()
        {
            if (isPassVisiable)
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

        public async Task Login()
        {
            int user_type = IsLeftOpened ? 2 : 1;
            try
            {
                var response = await Http.PostAsJsonAsync($"api/users/login/{user_type}", UserModel);

                Snackbar.Clear();
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Snackbar.Add("Unjeli ste pogrešne podatke!", Severity.Error);
                }
                else
                {
                    User returnValue = await response.Content.ReadFromJsonAsync<User>();
                    User = returnValue;
                    await LocalStorage.SetItemAsync("user", returnValue);
                    Snackbar.Add("Success!", Severity.Success);
                    await MessagesService.ConnectToServer(User, MessagesUpdateService, Snackbar);
                    Navigation.NavigateTo("/index");
                }
            }
            catch (HttpRequestException ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
                throw;
            }
        }

    }
}
