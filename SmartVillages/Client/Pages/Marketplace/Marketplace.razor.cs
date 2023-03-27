using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Newtonsoft.Json;
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
    public class MarketplaceBase : ComponentBase
    {
        [Inject] ILocalStorageService LocalStorage { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public HttpClient Http { get; set; }
        [Inject] public IDialogService DialogService { get; set; }
        public string Search { get; set; }
        public bool SingleProductOpened { get; set; }
        public User User { get; set; } = new User();
        public bool OnlyForFarmer { get; set; }
        public List<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
        public List<ProductViewModel> LastProducts { get; set; } = new List<ProductViewModel>(); 
        public List<ProductViewModel> MostSoldProducts { get; set; } = new List<ProductViewModel>();
        public List<ProductViewModel> SearchedProducts { get; set; } = new List<ProductViewModel>();
        public bool CanOpenDialog { get; set; }
        public Product OpenedProduct { get; set; }
        public List<CartItem> Cart { get; set; } = new List<CartItem>();
        public bool CartOpened { get; set; }

        public bool Loaded { get; set; } = false;
        public bool MyOrdersOpened { get; set; }
        public bool IsSeachedEmpty { get; set; }

        protected override async Task OnInitializedAsync()
        {
            User = await LocalStorage.GetItemAsync<User>("user");
            OnlyForFarmer = User.UserType.UserTypeId == 2 ? true : false;
            //await GetProducts();
            await GetLastProducts();
            await GetMostSoldProducts();
            await GetCategories();
            var Container = await LocalStorage.GetItemAsync<List<CartItem>>("cart");
            if (Container != null)
                Cart = Container;
            else
                Cart = new List<CartItem>();

            StateHasChanged();
        }

        public void OpenItem(int id, bool isOpenCart = false, string fromwhere = "products")
        {
            CloseItem();
            if (isOpenCart)
                CartOpened = true;
            else
            {
                if (fromwhere == "products")
                    foreach (var p in Products)
                    {
                        if (p.Id == id)
                        {
                            OpenedProduct = p;
                            break;
                        }
                    }
                else if (fromwhere == "lastproducts")
                    foreach (var p in LastProducts)
                    {
                        if (p.Id == id)
                        {
                            OpenedProduct = p;
                            break;
                        }
                    }
                else if (fromwhere == "searchedproducts")
                    foreach (var p in SearchedProducts)
                    {
                        if (p.Id == id)
                        {
                            OpenedProduct = p;
                            break;
                        }
                    }
                else
                    foreach (var p in MostSoldProducts)
                    {
                        if (p.Id == id)
                        {
                            OpenedProduct = p;
                            break;
                        }
                    }

                SingleProductOpened = true;
            }
            StateHasChanged();
        }
        public void CloseItem()
        {
            CartOpened = false;
            SingleProductOpened = false;
            MyOrdersOpened = false;
        }

        public async Task GetCategories()
        {
            var response = await Http.GetAsync($"api/productcategories");
            if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                ProductCategories = await response.Content.ReadFromJsonAsync<List<ProductCategory>>();
                CanOpenDialog = true;
                StateHasChanged();
            }
        }

        public async Task GetProducts()
        {
            Products.Clear();
            IsSeachedEmpty = false;
            StateHasChanged();
            var response = await Http.GetAsync($"api/products");
            if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                Products = await response.Content.ReadFromJsonAsync<List<ProductViewModel>>();
                Loaded = true;
                StateHasChanged();
            }
        }

        public async Task GetLastProducts()
        {
            LastProducts.Clear();
            IsSeachedEmpty = false;
            StateHasChanged();
            var response = await Http.GetAsync($"api/products/getlast");
            if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                LastProducts = await response.Content.ReadFromJsonAsync<List<ProductViewModel>>();
                Loaded = true;
                StateHasChanged();
            }
        }

        public async Task GetMostSoldProducts()
        {
            MostSoldProducts.Clear();
            IsSeachedEmpty = false;
            StateHasChanged();
            var response = await Http.GetAsync($"api/products/getmostsold");
            if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                MostSoldProducts = await response.Content.ReadFromJsonAsync<List<ProductViewModel>>();
                Loaded = true;
                StateHasChanged();
            }
        }

        public async Task OpenDialog()
        {
            if (CanOpenDialog)
            {
                var parameters = new DialogParameters();
                parameters.Add("User", User);
                parameters.Add("AllCategories", ProductCategories);

                DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.Medium };

                var dialog = DialogService.Show<AddNewProductDialog>("Add New Product", parameters, maxWidth);
                var result = await dialog.Result;
                if (!result.Cancelled)
                {
                    Loaded = false;
                    //await GetProducts();
                    await GetLastProducts();
                    await GetMostSoldProducts();
                }
            }
        }

        public async Task UpdateOnCart()
        {
            Cart = await LocalStorage.GetItemAsync<List<CartItem>>("cart");
            StateHasChanged();
        }

        public async Task RemoveFromCart(int id)
        {
            var item = Cart.Where(c => c.Product.Id == id).FirstOrDefault();
            Cart.Remove(item);
            await LocalStorage.SetItemAsync("cart", Cart);
        }

        public async void OpenAddToChartDialog(int id, string fromwhere = "products")
        {
            var parameters = new DialogParameters();
            parameters.Add("CartList", Cart);
            if (fromwhere == "products")
                parameters.Add("Product", Products.Where(c => c.Id == id).FirstOrDefault());
            else if (fromwhere == "lastproducts")
                parameters.Add("Product", LastProducts.Where(c => c.Id == id).FirstOrDefault());
            else if (fromwhere == "searchedproducts")
                parameters.Add("Product", SearchedProducts.Where(c => c.Id == id).FirstOrDefault());
            else
                parameters.Add("Product", MostSoldProducts.Where(c => c.Id == id).FirstOrDefault());

            var dialog = DialogService.Show<AddToCartDialog>("Add to cart", parameters);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await UpdateOnCart();
            }
        }

        public async Task CloseCart()
        {
            await LocalStorage.RemoveItemAsync("cart");
            CartOpened = false;
            Cart.Clear();
            Loaded = false;
            //await GetProducts();
            await GetLastProducts();
            await GetMostSoldProducts();
        }

        public void OpenCloseMyOrders()
        {
            CloseItem();
            MyOrdersOpened = true;
            StateHasChanged();
        }

        public async Task DeletedOrEdited()
        {
            CloseItem();
            Loaded = false;
            //await GetProducts();
            await GetLastProducts();
            await GetMostSoldProducts();
        }

        public async Task SearchProducts(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                ClearLists();

                if (string.IsNullOrEmpty(Search) || string.IsNullOrWhiteSpace(Search))
                {
                    Loaded = false;
                    await GetLastProducts();
                    await GetMostSoldProducts();
                }
                else
                {
                    var response = await Http.GetAsync($"api/products/getsearch/{Search}");
                    if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                    {
                        SearchedProducts = await response.Content.ReadFromJsonAsync<List<ProductViewModel>>();
                        if (SearchedProducts.Count < 1)
                            IsSeachedEmpty = true;
                        else
                            IsSeachedEmpty = false;
                        Loaded = true;
                        StateHasChanged();
                    }
                }
            }
        }

        public async Task OpenFilterDialog()
        {
            var parameters = new DialogParameters();
            parameters.Add("User", User);
            parameters.Add("AllCategories", ProductCategories);
            DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            var dialog = DialogService.Show<OpenFilterMarketplaceDialog>("Filter products", parameters, maxWidth);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var filter = JsonConvert.DeserializeObject<FilterProducts>(result.Data.ToString());
                Console.WriteLine(result.Data.ToString());
                ClearLists();
                await FilterProducts(filter);
            }
        }

        public async Task FilterProducts(FilterProducts filter)
        {

            var response = await Http.PostAsJsonAsync($"api/products/getfilteredproducts", filter);
            if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                SearchedProducts = await response.Content.ReadFromJsonAsync<List<ProductViewModel>>();
                Loaded = true;
                StateHasChanged();
            }
        }

        public void ClearLists()
        {
            Loaded = false;
            Products.Clear();
            LastProducts.Clear();
            MostSoldProducts.Clear();
            SearchedProducts.Clear();
            StateHasChanged();
        }
    }
}
