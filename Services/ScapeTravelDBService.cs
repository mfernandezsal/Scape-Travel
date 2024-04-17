using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using ScapeTravelWEB.Data;
using ScapeTravelWEB.Models.ScapeTravelDB;

namespace ScapeTravelWEB
{
    public partial class ScapeTravelDBService
    {
        ScapeTravelDBContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly ScapeTravelDBContext context;
        private readonly NavigationManager navigationManager;

        public ScapeTravelDBService(ScapeTravelDBContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportClientesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/scapetraveldb/clientes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/scapetraveldb/clientes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportClientesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/scapetraveldb/clientes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/scapetraveldb/clientes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnClientesRead(ref IQueryable<ScapeTravelWEB.Models.ScapeTravelDB.Cliente> items);

        public async Task<IQueryable<ScapeTravelWEB.Models.ScapeTravelDB.Cliente>> GetClientes(Query query = null)
        {
            var items = Context.Clientes.AsQueryable();

            items = items.Include(i => i.Familia);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnClientesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnClienteGet(ScapeTravelWEB.Models.ScapeTravelDB.Cliente item);
        partial void OnGetClienteByCi(ref IQueryable<ScapeTravelWEB.Models.ScapeTravelDB.Cliente> items);


        public async Task<ScapeTravelWEB.Models.ScapeTravelDB.Cliente> GetClienteByCi(int id)
        {
            var items = Context.Clientes
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Familia);
 
            OnGetClienteByCi(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnClienteGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnClienteCreated(ScapeTravelWEB.Models.ScapeTravelDB.Cliente item);
        partial void OnAfterClienteCreated(ScapeTravelWEB.Models.ScapeTravelDB.Cliente item);

        public async Task<ScapeTravelWEB.Models.ScapeTravelDB.Cliente> CreateCliente(ScapeTravelWEB.Models.ScapeTravelDB.Cliente cliente)
        {
            OnClienteCreated(cliente);

            var existingItem = Context.Clientes
                              .Where(i => i.Id == cliente.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Clientes.Add(cliente);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(cliente).State = EntityState.Detached;
                throw;
            }

            OnAfterClienteCreated(cliente);

            return cliente;
        }

        public async Task<ScapeTravelWEB.Models.ScapeTravelDB.Cliente> CancelClienteChanges(ScapeTravelWEB.Models.ScapeTravelDB.Cliente item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnClienteUpdated(ScapeTravelWEB.Models.ScapeTravelDB.Cliente item);
        partial void OnAfterClienteUpdated(ScapeTravelWEB.Models.ScapeTravelDB.Cliente item);

        public async Task<ScapeTravelWEB.Models.ScapeTravelDB.Cliente> UpdateCliente(int id, ScapeTravelWEB.Models.ScapeTravelDB.Cliente cliente)
        {
            OnClienteUpdated(cliente);

            var itemToUpdate = Context.Clientes
                              .Where(i => i.Id  == cliente.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(cliente);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterClienteUpdated(cliente);

            return cliente;
        }

        partial void OnClienteDeleted(ScapeTravelWEB.Models.ScapeTravelDB.Cliente item);
        partial void OnAfterClienteDeleted(ScapeTravelWEB.Models.ScapeTravelDB.Cliente item);

        public async Task<ScapeTravelWEB.Models.ScapeTravelDB.Cliente> DeleteCliente(int id)
        {
            var itemToDelete = Context.Clientes
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnClienteDeleted(itemToDelete);


            Context.Clientes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterClienteDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportFamiliaToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/scapetraveldb/familia/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/scapetraveldb/familia/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportFamiliaToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/scapetraveldb/familia/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/scapetraveldb/familia/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnFamiliaRead(ref IQueryable<ScapeTravelWEB.Models.ScapeTravelDB.Familia> items);

        public async Task<IQueryable<ScapeTravelWEB.Models.ScapeTravelDB.Familia>> GetFamilia(Query query = null)
        {
            var items = Context.Familias.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnFamiliaRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnFamiliaGet(ScapeTravelWEB.Models.ScapeTravelDB.Familia item);
        partial void OnGetFamiliaById(ref IQueryable<ScapeTravelWEB.Models.ScapeTravelDB.Familia> items);


        public async Task<ScapeTravelWEB.Models.ScapeTravelDB.Familia> GetFamiliaById(int id)
        {
            var items = Context.Familias
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetFamiliaById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnFamiliaGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnFamiliaCreated(ScapeTravelWEB.Models.ScapeTravelDB.Familia item);
        partial void OnAfterFamiliaCreated(ScapeTravelWEB.Models.ScapeTravelDB.Familia item);

        public async Task<ScapeTravelWEB.Models.ScapeTravelDB.Familia> CreateFamilia(ScapeTravelWEB.Models.ScapeTravelDB.Familia familia)
        {
            OnFamiliaCreated(familia);

            var existingItem = Context.Familias
                              .Where(i => i.Id == familia.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Familias.Add(familia);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(familia).State = EntityState.Detached;
                throw;
            }

            OnAfterFamiliaCreated(familia);

            return familia;
        }

        public async Task<ScapeTravelWEB.Models.ScapeTravelDB.Familia> CancelFamiliaChanges(ScapeTravelWEB.Models.ScapeTravelDB.Familia item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnFamiliaUpdated(ScapeTravelWEB.Models.ScapeTravelDB.Familia item);
        partial void OnAfterFamiliaUpdated(ScapeTravelWEB.Models.ScapeTravelDB.Familia item);

        public async Task<ScapeTravelWEB.Models.ScapeTravelDB.Familia> UpdateFamilia(int id, ScapeTravelWEB.Models.ScapeTravelDB.Familia familia)
        {
            OnFamiliaUpdated(familia);

            var itemToUpdate = Context.Familias
                              .Where(i => i.Id == familia.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(familia);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterFamiliaUpdated(familia);

            return familia;
        }

        partial void OnFamiliaDeleted(ScapeTravelWEB.Models.ScapeTravelDB.Familia item);
        partial void OnAfterFamiliaDeleted(ScapeTravelWEB.Models.ScapeTravelDB.Familia item);

        public async Task<ScapeTravelWEB.Models.ScapeTravelDB.Familia> DeleteFamilia(int id)
        {
            var itemToDelete = Context.Familias
                              .Where(i => i.Id == id)
                              .Include(i => i.Clientes)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnFamiliaDeleted(itemToDelete);


            Context.Familias.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterFamiliaDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task<List<Cliente>> GetFamilyMembers(int familiaId)
        {
            var integrants = await Context.Clientes
                                    .Where(c => c.FamiliaId== familiaId)
                                    .ToListAsync();

            return integrants;
        }
    }
}