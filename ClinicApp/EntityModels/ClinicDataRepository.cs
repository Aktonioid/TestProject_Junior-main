using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.EntityModels
{
    //object of the class connects the dbContext with the code.
    //modify and delete functions are invoking synchronously, cuz 
    //they're launching when connection is already established and the db is initialized
    public class ClinicDataRepository
    {
        public async Task<IEnumerable<PatientCard>> GetPatientCards()
        {
            ClinicDbContext dbContext = new ClinicDbContext();
            return await Task<IEnumerable<PatientCard>>
                .Run(() => {
                    return new ObservableCollection<PatientCard>(dbContext.PatientCardDbSet
                    .Include("Requests"));
                });
        }
        public async Task<IEnumerable<PatientCard>> GetPatientCardsByName(String searchPattern)
        {
            ClinicDbContext dbContext = new ClinicDbContext();
            return await Task<IEnumerable<PatientCard>>
                .Run(() => {
                    return new ObservableCollection<PatientCard>(dbContext.PatientCardDbSet
                        .Include("Requests"))
                        .Where(p => p.Name.ToUpper()
                        .StartsWith(searchPattern.ToUpper()));
                });
        }
        public async Task<IEnumerable<Request>> GetPatientRequests(Int32 id)
        {
            ClinicDbContext dbContext = new ClinicDbContext();
            return await Task<IEnumerable<PatientCard>>
                .Run(() =>
                {
                    return new ObservableCollection<Request>(dbContext.RequestDbSet
                        .Include("Patient")
                        .Where(r => r.Patient.Id == id));
                });
        }
        public async Task<Boolean> AddPatientCard(PatientCard card)
        {
            ClinicDbContext dbContext = new ClinicDbContext();
            await Task.Run(() => dbContext.PatientCardDbSet.Add(card));
            return 0 < dbContext.SaveChanges();
        }
        public async Task<Boolean> AddRequest(Request request)
        {
            ClinicDbContext dbContext = new ClinicDbContext();
            await Task.Run(() => {
                PatientCard card = request.Patient;
                dbContext.Set(typeof(PatientCard)).Attach(card);
                dbContext.Entry(card).State = System.Data.Entity.EntityState.Modified;
                dbContext.RequestDbSet.Add(request);
            });
            return 0 < dbContext.SaveChanges();
        }
        public Boolean ModifyPatientCard(PatientCard patientCard)
        {
            ClinicDbContext dbContext = new ClinicDbContext();
            dbContext.Set(typeof(PatientCard)).Attach(patientCard);
            dbContext.Entry(patientCard).State = System.Data.Entity.EntityState.Modified;
            return 0 < dbContext.SaveChanges();
        }
        public Boolean ModifyRequest(Request request)
        {
            ClinicDbContext dbContext = new ClinicDbContext();
            PatientCard card = request.Patient;
            dbContext.Set(typeof(PatientCard)).Attach(card);
            dbContext.Entry(card).State = System.Data.Entity.EntityState.Modified;
            dbContext.Set(typeof(Request)).Attach(request);
            
            dbContext.Entry(request).State = System.Data.Entity.EntityState.Modified;
            return 0 < dbContext.SaveChanges();
        }
        public Boolean DeleteCard(Int32 id)
        {
            ClinicDbContext dbContext = new ClinicDbContext();
            var cardToDelete = dbContext.PatientCardDbSet.Where(p => p.Id == id).FirstOrDefault();
            if (cardToDelete != null)
            {
                dbContext.PatientCardDbSet.Remove(cardToDelete);
            }
            return 0 < dbContext.SaveChanges();
        }
        public Boolean DeleteRequest(Int32 id)
        {
            ClinicDbContext dbContext = new ClinicDbContext();
            var requestToDelete = dbContext.RequestDbSet.Where(r => r.RequestId == id).FirstOrDefault();
            if (requestToDelete != null)
            {
                dbContext.RequestDbSet.Remove(requestToDelete);
            }
            return 0 < dbContext.SaveChanges();
        }
    }
}
