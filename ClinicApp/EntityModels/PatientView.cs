using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.EntityModels
{
    public class PatientView
    {
        public PatientCard card { get; set; }
        public int Age { get; set; }
    }
}
