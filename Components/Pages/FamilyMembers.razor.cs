using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using ScapeTravelWEB.Models.ScapeTravelDB;
using Microsoft.AspNetCore.Components.Web;

namespace ScapeTravelWEB.Components.Pages
{
    public partial class FamilyMembers
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
        public List<Cliente> Integrants { get; set; }

        [Parameter]
        public string FamilyName { get; set; }

        private void closeDialog() {
            DialogService.Close();
        }
    }
}
