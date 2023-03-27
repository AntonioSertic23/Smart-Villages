using Microsoft.AspNetCore.Components;
using MudBlazor;
using SmartVillages.Shared.MarketplaceModels;
using SmartVillages.Shared.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SmartVillages.Client.Shared.Dialogs
{
    public class OpenActiveEndedMoreDetailsDialogBase : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public OrderViewModel Order { get; set; }
        [Parameter] public User User { get; set; }
        [Parameter] public bool IsForCustomerEnded { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public HttpClient Http { get; set; }
        public float MyPrice { get; set; } = 0;
        public bool Ordered { get; set; }
        public string Comment { get; set; } = "";
        public int SelectedVal { get; set; } = 0;
        public int? ActiveVal { get; set; }
        public int OpenedItem { get; set; }
        public bool isEdit { get; set; }
        public int RateID { get; set; }

        protected override Task OnInitializedAsync()
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;
            Snackbar.Configuration.SnackbarVariant = Variant.Filled;

            MyPrice = 0;
            foreach (var item in Order.CartItems)
            {
                if (item.Product.User.Id == User.Id)
                {
                    MyPrice += item.Price;
                }
                if (item.StatusCode > 1)
                {
                    Ordered = true;
                }
            }
            return base.OnInitializedAsync();
        }

        public void Cancel() => MudDialog.Cancel();

        public async Task Submit()
        {
            await Http.PostAsJsonAsync($"api/orders/setasordered/{User.Id}", Order);

            MudDialog.Close(DialogResult.Ok(true));
        }

        // RATING AND COMMENTING

        public void HandleHoveredValueChanged(int? val) => ActiveVal = val;
        public string LabelText => (ActiveVal ?? SelectedVal) switch
        {
            1 => "Very bad",
            2 => "Bad",
            3 => "Sufficient",
            4 => "Good",
            5 => "Awesome!",
            _ => "Rate our product!"
        };

        public async Task RateAndComment(Product product)
        {
            if (isEdit)
            {
                ProductRate productRate = new ProductRate { Id = RateID, Date = DateTime.Today, Comment = Comment, Rate = SelectedVal, User = User, Product = product };
                await Http.PutAsJsonAsync($"api/productrates", productRate);
                Snackbar.Clear();
                Snackbar.Add("Success!", Severity.Success);
            }
            else
            {
                ProductRate productRate = new ProductRate { Date = DateTime.Today, Comment = Comment, Rate = SelectedVal, User = User, Product = product };
                var response = await Http.PostAsJsonAsync("api/productrates", productRate);
                var rate = await response.Content.ReadFromJsonAsync<ProductRate>();
                isEdit = true;
                RateID = rate.Id;
                StateHasChanged();
                Snackbar.Clear();
                Snackbar.Add("Success!", Severity.Success);
                
            }
        }

        public async Task Fill(int id)
        {
            if(OpenedItem != id)
            {
                var response = await Http.GetAsync($"api/productrates/getratesbyuserandorder/{User.Id}/{id}");

                string content = await response.Content.ReadAsStringAsync();
                if (content != "") 
                {
                    var rate = await response.Content.ReadFromJsonAsync<ProductRate>();
                    Comment = rate.Comment;
                    SelectedVal = rate.Rate;
                    isEdit = true;
                    RateID = rate.Id;
                }
                else
                {
                    Comment = "";
                    SelectedVal = 0;
                    isEdit = false;
                }
                OpenedItem = id;
                StateHasChanged();
            }
        }

        public async Task DeleteRate()
        {
            await Http.DeleteAsync($"api/productrates/{RateID}");
            Snackbar.Clear();
            Snackbar.Add("Success!", Severity.Success);
            await Fill(RateID);
        }
    }
}
