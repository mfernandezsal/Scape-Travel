using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ScapeTravelWEB.Models.ScapeTravelDB;

namespace ScapeTravelWEB.Data
{
    public class ScapeTravelDBContext : DbContext
    {
        public ScapeTravelDBContext(DbContextOptions<ScapeTravelDBContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Familia> Familias { get; set; }
    }
}