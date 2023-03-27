using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SmartVillages.Client.Shared.Dialogs;
using SmartVillages.Shared.MarketplaceModels;
using SmartVillages.Shared.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SmartVillages.Client.Pages.Marketplace
{
    public class MyOrdersBase : ComponentBase
    {
        [Inject] public HttpClient Http { get; set; }
        [Inject] ILocalStorageService LocalStorage { get; set; }
        [Inject] public IDialogService DialogService { get; set; }
        public User User { get; set; }
        public List<OrderViewModel> MyOrders { get; set; } = new List<OrderViewModel>();
        public List<OrderViewModel> ActiveOrders { get; set; } = new List<OrderViewModel>();
        public List<OrderViewModel> EndedOrders { get; set; } = new List<OrderViewModel>();
        public bool IsFarmer { get; set; }

        protected override async Task OnInitializedAsync()
        {
            User = await LocalStorage.GetItemAsync<User>("user");
            IsFarmer = User.UserType.UserTypeId == 2 ? true : false;
            await GetMyOrders();
            if (IsFarmer)
            {
                await GetActiveOrders();
                await GetEndedOrders();
            }
            else
            {
                await GetEndedOrdersCustomer();
            }
        }

        public async Task GetMyOrders()
        {
            var response = await Http.GetAsync($"api/orders/getmyorders/{User.Id}");
            List<OrderViewModel> returnValue = await response.Content.ReadFromJsonAsync<List<OrderViewModel>>();
            MyOrders = returnValue;
            StateHasChanged();
        }

        public async Task GetActiveOrders()
        {
            ActiveOrders.Clear();
            var response = await Http.GetAsync($"api/orders/getactiveorders/{User.Id}");
            List<OrderViewModel> returnValue = await response.Content.ReadFromJsonAsync<List<OrderViewModel>>();
            ActiveOrders = returnValue;
            StateHasChanged();
        }

        public async Task GetEndedOrders()
        {
            EndedOrders.Clear();
            var response = await Http.GetAsync($"api/orders/getendedorders/{User.Id}");
            List<OrderViewModel> returnValue = await response.Content.ReadFromJsonAsync<List<OrderViewModel>>();
            EndedOrders = returnValue;
            StateHasChanged();
        }

        public void OpenDialog(OrderViewModel order)
        {
            var parameters = new DialogParameters();
            parameters.Add("Order", order);

            DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            DialogService.Show<OpenMyOrdersMoreDetailsDialog>("Order review", parameters, maxWidth);
        }

        public async Task OpenActiveEndedDialog(OrderViewModel item, bool isForCustomerEnded = false)
        {
            var parameters = new DialogParameters();
            parameters.Add("Order", item);
            parameters.Add("User", User);
            if (isForCustomerEnded)
                parameters.Add("IsForCustomerEnded", true);

            DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            var dialog = DialogService.Show<OpenActiveEndedMoreDetailsDialog>("Order review", parameters, maxWidth);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await GetActiveOrders();
                await GetEndedOrders();
            }
        }

        public async Task GetEndedOrdersCustomer()
        {
            EndedOrders.Clear();
            var response = await Http.GetAsync($"api/orders/getendedorderscustomer/{User.Id}");
            List<OrderViewModel> returnValue = await response.Content.ReadFromJsonAsync<List<OrderViewModel>>();
            EndedOrders = returnValue;
            StateHasChanged();
        }

    }
}
