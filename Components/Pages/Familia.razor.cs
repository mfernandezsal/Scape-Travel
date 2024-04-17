using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using ScapeTravelWEB.Models.ScapeTravelDB;

namespace ScapeTravelWEB.Components.Pages
{
    public partial class Familia
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

        protected IEnumerable<ScapeTravelWEB.Models.ScapeTravelDB.Familia> familia;

        protected RadzenDataGrid<ScapeTravelWEB.Models.ScapeTravelDB.Familia> grid0;

        protected string search = "";

        protected List<Cliente> integrants = new List<Cliente>();

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            familia = await ScapeTravelDBService.GetFamilia(new Query { Filter = $@"i => i.Nombre.Contains(@0)", FilterParameters = new object[] { search } });
        }
        protected override async Task OnInitializedAsync()
        {
            familia = await ScapeTravelDBService.GetFamilia(new Query { Filter = $@"i => i.Nombre.Contains(@0)", FilterParameters = new object[] { search } });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddFamilia>("Agregar Familia", null);
            await grid0.Reload();
        }

        protected async Task EditRow(MouseEventArgs args, ScapeTravelWEB.Models.ScapeTravelDB.Familia familia)
        {
            await DialogService.OpenAsync<EditFamilia>("Editar Familia", new Dictionary<string, object> { {"Id", familia.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, ScapeTravelWEB.Models.ScapeTravelDB.Familia familia)
        {
            try
            {
                var confirmOptions = new ConfirmOptions
                {
                    OkButtonText = "Eliminar",
                    CancelButtonText = "Salir",
                };

                if (await DialogService.Confirm($"¿Desea eliminar la familia {familia.Nombre}?", "Eliminar familia", confirmOptions) == true)
                {
                    var deleteResult = await ScapeTravelDBService.DeleteFamilia(familia.Id);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Familia"
                });
            }
        }

        protected async Task FindAllIntegrants(MouseEventArgs args, ScapeTravelWEB.Models.ScapeTravelDB.Familia familia)
        {

            try
            {
                integrants = await ScapeTravelDBService.GetFamilyMembers(familia.Id);
                await DialogService.OpenAsync<FamilyMembers>(string.Empty, new Dictionary<string, object> { { "Integrants", integrants }, { "FamilyName", familia.Nombre} },
                    new DialogOptions() { Width = "700px", Height = "500px", Resizable = true, Draggable = true, ShowTitle=false});
                await grid0.Reload();

            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se pudo cargar la familia"
                });
            }
        }
    }
}