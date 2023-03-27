using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SmartVillages.Client.Shared.Dialogs;
using SmartVillages.Shared;
using SmartVillages.Shared.MarketplaceModels;
using SmartVillages.Shared.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SmartVillages.Client.Pages
{
    public class SingleProductBase : ComponentBase
    {
        [Parameter] public Product Product { get; set; }
        [Parameter] public User User { get; set; }
        [Parameter] public List<ProductCategory> ProductCategories { get; set; }
        [Parameter] public EventCallback ChartCallback { get; set; }
        [Parameter] public EventCallback DeletedOrEdited { get; set; }

        [Inject] ILocalStorageService LocalStorage { get; set; }
        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public HttpClient Http { get; set; }

        public bool Addedornot { get; set; }
        public string LinkToProfile { get; set; }
        public string LinkToMessages { get; set; }
        public List<CartItem> Cart { get; set; } = new List<CartItem>();
        public List<ProductRate> Rates { get; set; } = new List<ProductRate>();
        public float GlobalRate { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            var cart = await LocalStorage.GetItemAsync<List<CartItem>>("cart");
            if(cart != null)
            {
                Cart = cart;
                Addedornot = Cart.Where(c => c.Product.Id == Product.Id).FirstOrDefault() != null ? true : false;
            }
            LinkToProfile = "/profile/" + Product.User.Id;
            LinkToMessages = "/messages/" + Product.User.Id;

            await GetRates();
        }

        public async Task Edit()
        {
            var parameters = new DialogParameters();
            var product = Product;
            parameters.Add("Product", product);
            parameters.Add("AllCategories", ProductCategories);
            DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            var dialog = DialogService.Show<EditProductDialog>("Edit product", parameters, maxWidth);
            await dialog.Result;
        }

        public async Task Delete()
        {
            var dialog = DialogService.Show<DeleteProductDialog>("Delete product");
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var response = await Http.DeleteAsync($"api/products/{Product.Id}");
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    await DeletedOrEdited.InvokeAsync();
                }
            }
        }

        public async Task RemoveFromCart()
        {
            var item = Cart.Where(c => c.Product.Id == Product.Id).FirstOrDefault();
            Cart.Remove(item);
            Addedornot = !Addedornot;
            StateHasChanged();
            await LocalStorage.SetItemAsync("cart", Cart);
            await ChartCallback.InvokeAsync();
        }

        public async void OpenAddToChartDialog()
        {
            var parameters = new DialogParameters();
            parameters.Add("Product", Product);
            parameters.Add("CartList", Cart);

            var dialog = DialogService.Show<AddToCartDialog>("Add to cart", parameters);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                Cart = await LocalStorage.GetItemAsync<List<CartItem>>("cart");
                await ChartCallback.InvokeAsync();
            }
        }

        public async Task GetRates()
        {
            var response = await Http.GetAsync($"api/productrates/getlastrates/{Product.Id}");

            string content = await response.Content.ReadAsStringAsync();
            if (content != "")
            {
                Rates = await response.Content.ReadFromJsonAsync<List<ProductRate>>();
                if(Rates.Count > 0)
                {
                    if(Rates.Count > 1)
                    {
                        int count = 0;
                        foreach (var r in Rates)
                            count += r.Rate;
                        GlobalRate = (float)count / (float)Rates.Count();
                    }
                    else
                    {
                        GlobalRate = Rates[0].Rate;
                    }
                }
                else
                {
                    GlobalRate = 0;
                }
                StateHasChanged();
            }
        }
    }
}
