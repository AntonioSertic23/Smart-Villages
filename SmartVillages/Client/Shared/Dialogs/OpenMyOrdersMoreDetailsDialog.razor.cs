using Microsoft.AspNetCore.Components;
using MudBlazor;
using SmartVillages.Shared.MarketplaceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartVillages.Client.Shared.Dialogs
{
    public class OpenMyOrdersMoreDetailsDialogBase : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public OrderViewModel Order { get; set; }

        public void Cancel() => MudDialog.Cancel();
    }
}
