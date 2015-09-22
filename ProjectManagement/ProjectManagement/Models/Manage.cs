using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class Manage: DbContext
    {
        public DbSet<Projects> project { get; set; }
        public DbSet<Location> location { get; set; }
    }
}