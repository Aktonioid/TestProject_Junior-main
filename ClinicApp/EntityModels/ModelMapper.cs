using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.EntityModels
{
    public static class ModelMapper
    {
        public static PatientView AsView(this PatientCard patientCard) 
        {
            var patientAge = DateTime.Now.Year - patientCard.DateOfBirth.Year;

            if (patientCard.DateOfBirth > DateTime.Now.AddYears(-patientAge))
            {
                patientAge--;
            }
            return new PatientView
            {
                card = patientCard,
                Age = patientAge
            };
        }
    }
}
