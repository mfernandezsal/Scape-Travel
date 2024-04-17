using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using ScapeTravelWEB.Data;

namespace ScapeTravelWEB.Components.Pages
{
    public partial class EditCliente
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

        protected FormOptions formOptions = new();

        protected override async Task OnInitializedAsync()
        {
            cliente = await ScapeTravelDBService.GetClienteByCi(Id);

            familiaForIdFamilia = await ScapeTravelDBService.GetFamilia();
        }
        protected bool errorVisible;
        protected ScapeTravelWEB.Models.ScapeTravelDB.Cliente cliente;

        protected IEnumerable<ScapeTravelWEB.Models.ScapeTravelDB.Familia> familiaForIdFamilia;

        protected async Task FormSubmit()
        {
            try
            {
                await ScapeTravelDBService.UpdateCliente(Id, cliente);
                DialogService.Close(cliente);
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