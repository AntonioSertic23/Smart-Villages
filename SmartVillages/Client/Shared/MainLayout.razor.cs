using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using SmartVillages.Shared;
using SmartVillages.Shared.UserModels;
using System.Net.Http;
using System.Net.Http.Json;
using SmartVillages.Client.Services;
using MudBlazor;

namespace SmartVillages.Client.Shared
{
    public class MainLayoutBase : LayoutComponentBase
    {
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] ILocalStorageService LocalStorage { get; set; }
        [Inject] public HttpClient Http { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public MessagesUpdateService MessagesUpdateService { get; set; }
        [Inject] public MessagesService MessagesService { get; set; }
        public User User { get; set; } = new User();

        public string FullName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var user = await LocalStorage.GetItemAsync<User>("user");
            if (user == null)
            {
                NavigationManager.NavigateTo("/");
            }
            else
            { 
                User = user;
                FullName = User.FirstName + " " + User.LastName;
            }

            await MessagesService.ConnectToServer(User, MessagesUpdateService, Snackbar);
        }

        public async Task Logout()
        {
            await LocalStorage.RemoveItemAsync("user");
            await LocalStorage.RemoveItemAsync("cart");
            await Http.PutAsJsonAsync($"api/userconnections/PutUserConnection", User.Id);
            NavigationManager.NavigateTo("/");
        }

    }
}
