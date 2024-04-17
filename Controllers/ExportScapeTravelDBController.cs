using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using ScapeTravelWEB.Data;

namespace ScapeTravelWEB.Controllers
{
    public partial class ExportScapeTravelDBController : ExportController
    {
        private readonly ScapeTravelDBContext context;
        private readonly ScapeTravelDBService service;

        public ExportScapeTravelDBController(ScapeTravelDBContext context, ScapeTravelDBService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/ScapeTravelDB/clientes/csv")]
        [HttpGet("/export/ScapeTravelDB/clientes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportClientesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetClientes(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ScapeTravelDB/clientes/excel")]
        [HttpGet("/export/ScapeTravelDB/clientes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportClientesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetClientes(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ScapeTravelDB/familia/csv")]
        [HttpGet("/export/ScapeTravelDB/familia/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportFamiliaToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetFamilia(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ScapeTravelDB/familia/excel")]
        [HttpGet("/export/ScapeTravelDB/familia/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportFamiliaToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetFamilia(), Request.Query, false), fileName);
        }
    }
}
