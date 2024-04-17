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
    public partial class Clientes
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

        protected IEnumerable<ScapeTravelWEB.Models.ScapeTravelDB.Cliente> clientes;

        protected RadzenDataGrid<ScapeTravelWEB.Models.ScapeTravelDB.Cliente> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            clientes = await ScapeTravelDBService.GetClientes(new Query { Filter = $@"i => i.Nombre.Contains(@0) || i.Apellido_pat.Contains(@0) || i.Apellido_mat.Contains(@0) || i.Genero.Contains(@0) || i.Nacionalidad.Contains(@0) || i.Nro_pasaporte.Contains(@0) || i.Telefono.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Familia" });
        }
        protected override async Task OnInitializedAsync()
        {
            clientes = await ScapeTravelDBService.GetClientes(new Query { Filter = $@"i => i.Nombre.Contains(@0) || i.Apellido_pat.Contains(@0) || i.Apellido_mat.Contains(@0) || i.Genero.Contains(@0) || i.Nacionalidad.Contains(@0) || i.Nro_pasaporte.Contains(@0) || i.Telefono.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Familia" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddCliente>("Agregar Cliente", null);
            await grid0.Reload();
        }

        protected async Task EditRow(MouseEventArgs args, ScapeTravelWEB.Models.ScapeTravelDB.Cliente cliente)
        {
            await DialogService.OpenAsync<EditCliente>("Editar Cliente", new Dictionary<string, object> { {"Id", cliente.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, ScapeTravelWEB.Models.ScapeTravelDB.Cliente cliente)
        {
            try
            {
                var confirmOptions = new ConfirmOptions
                {
                    OkButtonText = "Eliminar",
                    CancelButtonText = "Salir",
                };

                if (await DialogService.Confirm($"¿Desea eliminar a {cliente.Nombre}?", "Eliminar cliente", confirmOptions) == true)
                {
                    var deleteResult = await ScapeTravelDBService.DeleteCliente(cliente.Id);

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
                    Detail = $"No fue posible eliminar al cliente"
                });
            }
        }
    }
}