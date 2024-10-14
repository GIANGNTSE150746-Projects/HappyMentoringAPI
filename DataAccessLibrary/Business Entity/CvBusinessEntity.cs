using BusinessObjectLibrary;
using DataAccessLibrary.Interfaces;
using HmsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Business_Entity
{
    public class CvBusinessEntity
    {
        private IUnitOfWork work;
        public CvBusinessEntity(IUnitOfWork work)
        {
            this.work = work;
        }
        public async Task<IEnumerable<Cv>> GetCvsAsync()
        {
            return (await work.Cvs.GetAllAsync())
                .Where(cv => !cv.IsDeleted);
        }

        public async Task<Cv> GetCvAsync(string id)
        {
            return (await work.Cvs.GetAllAsync())
                .Where(cv => !cv.IsDeleted && cv.Id.Equals(id))
                .FirstOrDefault();
        }

        public async Task<Cv> AddCv(Cv cv)
        {
            string id = HmsUtils.CreateGuid();
            cv.Id = id;
            cv.IsDeleted = false;
            cv.CreatedDate = DateTime.Now;
            cv.Description = cv.Description;
            cv.MentorId = cv.MentorId;
            cv.Mentor = null;
            await work.Cvs.AddAsync(cv);
            await work.Save();
            return cv;
        }

        public async Task<Cv> UpdateCv(Cv cv)
        {
            work.Cvs.Update(cv);
            await work.Save();
            return cv;
        }

        public async Task RemoveCv(string id)
        {
            Cv cv = await work.Cvs.GetAsync(id);
            cv.IsDeleted = true;
            work.Cvs.Update(cv);
            await work.Save();
        }
    }
}
