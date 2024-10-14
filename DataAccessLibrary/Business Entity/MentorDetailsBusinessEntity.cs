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
    public class MentorDetailsBusinessEntity
    {
        private IUnitOfWork work;

        public MentorDetailsBusinessEntity(IUnitOfWork work)
        {
            this.work = work;
        }
        public async Task<IEnumerable<MentorDetail>> GetMentorDetailsAsync()
        {
            return await work.MentorDetails.GetAllAsync();
        }

        public async Task<MentorDetail> GetMentorDetailAsync(string MentorId)
        {
            return await work.MentorDetails.GetAsync(MentorId);
        }

        public async Task<MentorDetail> AddMentorDetail(MentorDetail mentorDetail)
        {
            await work.MentorDetails.AddAsync(mentorDetail);
            await work.Save();
            return mentorDetail;
        }

        public async Task<MentorDetail> UpdateMentorDetail(MentorDetail mentorDetail)
        {
            work.MentorDetails.Update(mentorDetail);
            await work.Save();
            return mentorDetail;
        }
    }
}
