using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace ScapeTravelWEB.Components.Pages
{
    public partial class EditFamilia
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }
        [Inject]
        public ScapeTravelDBService ScapeTravelDBService { get; set; }

        [Parameter]
        public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            familia = await ScapeTravelDBService.GetFamiliaById(Id);
        }
        protected bool errorVisible;
        protected ScapeTravelWEB.Models.ScapeTravelDB.Familia familia;

        protected async Task FormSubmit()
        {
            try
            {
                await ScapeTravelDBService.UpdateFamilia(Id, familia);
                DialogService.Close(familia);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}