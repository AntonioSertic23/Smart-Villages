using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SmartVillages.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartVillages.Shared.UserModels;

namespace SmartVillages.Client.Pages.UserSign
{
    public class HomeBase : ComponentBase
    {
        [Inject] IJSRuntime JsRuntime { get; set; } 
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] ILocalStorageService LocalStorage { get; set; }
        public bool LeftSignInOpened { get; set; }
        public bool LeftSignUpOpened { get; set; }
        public bool RightSignInOpened { get; set; }
        public bool RightSignUpOpened { get; set; }
        public bool IsLeftOpened { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var user = await LocalStorage.GetItemAsync<User>("user");
            if (user != null)
            {
                Navigation.NavigateTo("/index");
            }
        }

        public void OpenLeftSignIn()
        {
            CloseAll();
            LeftSignInOpened = true;
            IsLeftOpened = false;
            GoUp();
            StateHasChanged();
        }

        public void OpenLeftSignUp()
        {
            CloseAll();
            LeftSignUpOpened = true;
            IsLeftOpened = false;
            GoUp();
            StateHasChanged();
        }

        public void OpenRightSignIn()
        {
            CloseAll();
            RightSignInOpened = true;
            IsLeftOpened = true;
            GoDown();
            StateHasChanged();
        }

        public void OpenRightSignUp()
        {
            CloseAll();
            RightSignUpOpened = true;
            IsLeftOpened = true;
            GoDown();
            StateHasChanged();
        }

        public void CloseAll()
        {
            LeftSignInOpened = false;
            LeftSignUpOpened = false;
            RightSignInOpened = false;
            RightSignUpOpened = false;
        }

        public void GoBackPressed()
        {
            CloseAll();
            StateHasChanged();
        }

        public Task GoUp()
        {
            JsRuntime.InvokeVoidAsync("scrollToElementId", "top-contact");
            return Task.CompletedTask;
        }
        public Task GoDown()
        {
            JsRuntime.InvokeVoidAsync("scrollToElementId", "bottom-contact");
            return Task.CompletedTask;
        }

        public void OpenSignIn()
        {
            if (IsLeftOpened)
                OpenRightSignIn();
            else
                OpenLeftSignIn();
        }

        public void OpenSignUp()
        {
            if (IsLeftOpened)
                OpenRightSignUp();
            else
                OpenLeftSignUp();
        }
    }
}
