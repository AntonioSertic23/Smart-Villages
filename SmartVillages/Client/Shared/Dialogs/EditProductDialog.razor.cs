using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MoreLinq;
using MudBlazor;
using SmartVillages.Shared.MarketplaceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SmartVillages.Client.Shared.Dialogs
{
    public class EditProductDialogBAse : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public Product Product { get; set; }
        [Parameter] public List<ProductCategory> AllCategories { get; set; }
        [Inject] public HttpClient Http { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }

        public Product ProductModal { get; set; } = new Product();
        public ProductCategory ProductCategoryModal { get; set; } = new ProductCategory();

        public List<string> Categories { get; set; } = new List<string>();
        public List<string> SubCategoriesOne { get; set; } = new List<string>();
        public List<string> SubCategoriesTwo { get; set; } = new List<string>();
        public List<string> Species { get; set; } = new List<string>();

        
        public string Title { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public ProductImage ProductImage { get; set; } = new ProductImage();
        public bool Eco { get; set; }
        public float Quantity { get; set; }

        protected override Task OnInitializedAsync()
        {
            Categories = AllCategories.DistinctBy(x => x.Name).Select(s => s.Name).ToList();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;
            Snackbar.Configuration.SnackbarVariant = Variant.Filled;

            ProductModal = Product;
            Initialize();
            return base.OnInitializedAsync();
        }

        public void Initialize()
        {

            ProductCategoryModal.Name = Product.ProductCategory.Name;

            foreach (var i in AllCategories.Where(x => x.Name == ProductCategoryModal.Name).DistinctBy(d => d.SubCategoryOne))
                SubCategoriesOne.Add(i.SubCategoryOne);
            ProductCategoryModal.SubCategoryOne = Product.ProductCategory.SubCategoryOne;

            foreach (var i in AllCategories.Where(x => x.SubCategoryOne == ProductCategoryModal.SubCategoryOne).DistinctBy(d => d.SubCategoryTwo))
                SubCategoriesTwo.Add(i.SubCategoryTwo);
            ProductCategoryModal.SubCategoryTwo = Product.ProductCategory.SubCategoryTwo;

            foreach (var i in AllCategories.Where(x => x.SubCategoryTwo == ProductCategoryModal.SubCategoryTwo).DistinctBy(d => d.Species))
                Species.Add(i.Species);
            ProductCategoryModal.Species = Product.ProductCategory.Species;

            Title = Product.Title;
            Price = Convert.ToSingle(Product.Price);
            Description = Product.Description;
            ProductImage.Image = Product.ProductImage.Image;
            Eco = Product.Eco;
            Quantity = Convert.ToSingle(Product.Quantity);
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

        public void Cancel() 
        {
            Product.Title = Title;
            Product.Price = Price;
            Product.Description = Description;
            Product.ProductImage.Image = ProductImage.Image;
            Product.Eco = Eco;
            Product.Quantity = Quantity;

            MudDialog.Cancel();
        } 

        public async Task UploadFiles(InputFileChangeEventArgs e)
        {
            var entries = e.GetMultipleFiles();
            //get the file
            var file = e.File;
            //read that file in a byte array
            var buffer = new byte[file.Size];
            await file.OpenReadStream(1512000).ReadAsync(buffer);
            //convert byte array to base 64 string
            ProductModal.ProductImage.Image = $"data:image/png;base64,{Convert.ToBase64String(buffer)}";
            StateHasChanged();
        }

        public void HandleInvalidSubmit()
        {
            Snackbar.Clear();
            Snackbar.Add("Required fields must be filled!", Severity.Error);
        }

        public async Task EditProduct()
        {
            Snackbar.Clear();
            Snackbar.Add("Validateing..", Severity.Success);
            //validacija
            if (!string.IsNullOrEmpty(ProductCategoryModal.Name) && !string.IsNullOrEmpty(ProductCategoryModal.SubCategoryOne) && !string.IsNullOrEmpty(ProductCategoryModal.SubCategoryTwo) && !string.IsNullOrEmpty(ProductCategoryModal.Species))
            {
                try
                {
                    ProductModal.ProductCategory = ProductCategoryModal;
                    ProductModal.User = Product.User;

                    //image?
                    if (ProductModal.ProductImage.Image != ProductImage.Image)
                    {
                        var responseone = await Http.PostAsJsonAsync($"api/productimages", ProductModal.ProductImage);
                        var returnValueone = await responseone.Content.ReadFromJsonAsync<ProductImage>();
                        ProductModal.ProductImage = returnValueone;
                    }
                    else
                    {
                        ProductModal.ProductImage = Product.ProductImage;
                    }

                    //product
                    var response = await Http.PutAsJsonAsync($"api/products", ProductModal);
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
    }
}
