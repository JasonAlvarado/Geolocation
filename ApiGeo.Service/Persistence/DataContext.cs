using ApiGeo.Service.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGeo.Service.Persistence
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<GeoRequest> GeoRequests { get; set; }
        public DbSet<GeoResult> GeoResults { get; set; }
    }
}
