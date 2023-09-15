using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.EntityModels
{
    public enum TypeOfRequest {
        [Description("Первичный прием")]
        FirstRequest,
        [Description("Повторный прием")]
        SecondRequest
    };
    public class Request
    {
        public Int32 RequestId { get; set; }
        public DateTime DateOfRequest { get; set; }
        public TypeOfRequest RequestType { get; set; }
        public String Purpose { get; set; }
        [Required]
        public PatientCard Patient { get; set; }
    }
}
