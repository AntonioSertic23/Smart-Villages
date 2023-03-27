using Microsoft.AspNetCore.Components;
using MoreLinq;
using MudBlazor;
using SmartVillages.Shared.MarketplaceModels;
using SmartVillages.Shared.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartVillages.Client.Shared.Dialogs
{
    public class OpenFilterMarketplaceDialogBase : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public User User { get; set; }
        [Parameter] public List<ProductCategory> AllCategories { get; set; }
        [Inject] public HttpClient Http { get; set; }
        public FilterProducts FilterProducts { get; set; } = new FilterProducts();
        public ProductCategory ProductCategoryModal { get; set; } = new ProductCategory();
        public List<string> Categories { get; set; } = new List<string>();
        public List<string> SubCategoriesOne { get; set; } = new List<string>();
        public List<string> SubCategoriesTwo { get; set; } = new List<string>();
        public List<string> Species { get; set; } = new List<string>();

        protected override Task OnParametersSetAsync()
        {
            Categories = AllCategories.DistinctBy(x => x.Name).Select(s => s.Name).ToList();
            return base.OnParametersSetAsync();
        }

        public void Sort(int id)
        {
            if (id == 1)
            {
                SubCategoriesOne.Clear();
                ProductCategoryModal.SubCategoryOne = "";
                SubCategoriesTwo.Clear();
                ProductCategoryModal.SubCategoryTwo = "";
                Species.Clear();
                ProductCategoryModal.Species = "";
                foreach (var i in AllCategories.Where(x => x.Name == ProductCategoryModal.Name).DistinctBy(d => d.SubCategoryOne))
                {
                    SubCategoriesOne.Add(i.SubCategoryOne);
                }
            }
            if (id == 2)
            {
                SubCategoriesTwo.Clear();
                ProductCategoryModal.SubCategoryTwo = "";
                Species.Clear();
                ProductCategoryModal.Species = "";
                foreach (var i in AllCategories.Where(x => x.SubCategoryOne == ProductCategoryModal.SubCategoryOne).DistinctBy(d => d.SubCategoryTwo))
                {
                    SubCategoriesTwo.Add(i.SubCategoryTwo);
                }
            }
            if (id == 3)
            {
                Species.Clear();
                ProductCategoryModal.Species = "";
                foreach (var i in AllCategories.Where(x => x.SubCategoryTwo == ProductCategoryModal.SubCategoryTwo).DistinctBy(d => d.Species))
                {
                    Species.Add(i.Species);
                }
            }
        }

        public void Cancel() => MudDialog.Cancel();

        public async Task Submit()
        {
            FilterProducts.ProductCategory = ProductCategoryModal;
            string jsonString = JsonSerializer.Serialize(FilterProducts);

            MudDialog.Close(DialogResult.Ok(jsonString));
        }
    }
}
