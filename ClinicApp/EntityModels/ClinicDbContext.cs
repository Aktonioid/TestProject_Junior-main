using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.EntityModels
{
    class ClinicDbContext: DbContext
    {
        public DbSet<PatientCard> PatientCardDbSet { get; set; }
        public DbSet<Request> RequestDbSet { get; set; }
    }
}
