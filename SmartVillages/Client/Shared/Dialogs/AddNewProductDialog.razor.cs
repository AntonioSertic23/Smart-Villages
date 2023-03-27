using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MoreLinq;
using MudBlazor;
using SmartVillages.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SmartVillages.Shared.UserModels;
using SmartVillages.Shared.MarketplaceModels;

namespace SmartVillages.Client.Shared.Dialogs
{
    public class AddNewProductDialogBase : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public User User { get; set; }
        [Parameter] public List<ProductCategory> AllCategories { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public HttpClient Http { get; set; }

        public Product ProductModal { get; set; } = new Product();
        public ProductCategory ProductCategoryModal { get; set; } = new ProductCategory();

        public List<string> Categories { get; set; } = new List<string>();
        public List<string> SubCategoriesOne { get; set; } = new List<string>();
        public List<string> SubCategoriesTwo { get; set; } = new List<string>();
        public List<string> Species { get; set; } = new List<string>();

        protected override Task OnParametersSetAsync()
        {
            Categories = AllCategories.DistinctBy(x => x.Name).Select(s => s.Name).ToList();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;
            Snackbar.Configuration.SnackbarVariant = Variant.Filled;
            ProductModal.ProductImage = new ProductImage();
            return base.OnParametersSetAsync();
        }

        public void Sort(int id)
        {
            if(id == 1)
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
            if(id == 2)
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

        public void HandleInvalidSubmit()
        {
            Snackbar.Clear();
            Snackbar.Add("Required fields must be filled!", Severity.Error);
        }

        public async Task AddProduct()
        {
            //validacija
            if (!string.IsNullOrEmpty(ProductCategoryModal.Name) && !string.IsNullOrEmpty(ProductCategoryModal.SubCategoryOne) && !string.IsNullOrEmpty(ProductCategoryModal.SubCategoryTwo) && !string.IsNullOrEmpty(ProductCategoryModal.Species))
            {
                //ubacivanje
                Snackbar.Clear();
                Snackbar.Add("Validateing..", Severity.Success);
                try
                {
                    ProductModal.ProductCategory = ProductCategoryModal;
                    ProductModal.User = User;

                    var responseone = await Http.PostAsJsonAsync($"api/productimages", ProductModal.ProductImage);
                    ProductImage returnValue = await responseone.Content.ReadFromJsonAsync<ProductImage>();

                    ProductModal.ProductImage = returnValue;
                    var response = await Http.PostAsJsonAsync($"api/products", ProductModal);
                    if (response.StatusCode != System.Net.HttpStatusCode.InternalServerError)
                    {
                        Snackbar.Clear();
                        Snackbar.Add("Success!", Severity.Success);
                        MudDialog.Close(DialogResult.Ok(true));
                    }
                }
                catch (HttpRequestException ex)
                {
                    Snackbar.Clear();
                    Snackbar.Add("Error: " + ex.Message, Severity.Success);
                }
            }
            else
            {
                Snackbar.Add("All category fields must be filled", Severity.Error);
            }
        }

        public void Cancel() => MudDialog.Cancel();

        public async Task UploadFiles(InputFileChangeEventArgs e)
        {
            var entries = e.GetMultipleFiles();
            //validations
            /*
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add($"Files with {entries.FirstOrDefault().Size} bytes size are not allowed", Severity.Error);
            Snackbar.Add($"Files starting with letter {entries.FirstOrDefault().Name.Substring(0, 1)} are not recommended", Severity.Warning);
            Snackbar.Add($"This file has the extension {entries.FirstOrDefault().Name.Split(".").Last()}", Severity.Info);
            */
            //get the file
            var file = e.File;
            //read that file in a byte array
            var buffer = new byte[file.Size];
            await file.OpenReadStream(1512000).ReadAsync(buffer);
            //convert byte array to base 64 string
            ProductModal.ProductImage.Image = $"data:image/png;base64,{Convert.ToBase64String(buffer)}";
            StateHasChanged();
        }
    }
}
