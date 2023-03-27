using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SmartVillages.Shared;
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
    public class CartOrderBase : ComponentBase
    {
        [Parameter] public EventCallback CartUpdate { get; set; }
        [Parameter] public EventCallback CloseCart { get; set; }
        [Inject] ILocalStorageService LocalStorage { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public HttpClient Http { get; set; }
        public float Weight { get; set; }
        public float FinalCartPrice { get; set; }
        public MudTabs tabs { get; set; }
        public int Count { get; set; } = 0;
        public List<CartItem> Cart { get; set; } = new List<CartItem>();
        public User User { get; set; }
        public DateTime? date { get; set; } = DateTime.Today.AddDays(2);
        public DateTime? mindate { get; set; } = DateTime.Today.AddDays(2);
        public DateTime? maxdate { get; set; } = DateTime.Today.AddDays(30);
        public Order Order { get; set; } = new Order();

        protected override async Task OnInitializedAsync()
        {
            Cart = await LocalStorage.GetItemAsync<List<CartItem>>("cart");
            User = await LocalStorage.GetItemAsync<User>("user");
            FinalCartPrice = Cart.Select(s => s.Price).Sum();
        }

        public void Reset()
        {
            Count = 0;
        }

        public void Activate(int index)
        {
            Reset();
            tabs.ActivatePanel(index);
        }

        public async Task NewPrice(int id)
        {
            Count++;
            if(Count > Cart.Count())
            {
                CartItem Item = Cart.Where(c => c.Product.Id == id).FirstOrDefault();
                Item.Price = Item.Quantity * Convert.ToSingle(Item.Product.Price);
                FinalCartPrice = Cart.Select(s => s.Price).Sum();
                await LocalStorage.SetItemAsync("cart", Cart);
                await CartUpdate.InvokeAsync();
            }
        }

        public async Task RemoveFromCart(CartItem item)
        {
            Cart.Remove(item);
            await LocalStorage.SetItemAsync("cart", Cart);
            await CartUpdate.InvokeAsync();
        }

        public async Task CreateNewOrder()
        {
            Order.Buyer = User;
            Order.FromDate = DateTime.Today;
            Order.Price = FinalCartPrice;
            Order.ToDate = date.Value;

            var response = await Http.PostAsJsonAsync("api/orders", Order);
            Order returnValue = await response.Content.ReadFromJsonAsync<Order>();

            foreach(var item in Cart)
            {
                item.OrderId = returnValue.Id;
                await Http.PostAsJsonAsync("api/cartitems", item);
            }

            await CloseCart.InvokeAsync();
        }
    }
}
