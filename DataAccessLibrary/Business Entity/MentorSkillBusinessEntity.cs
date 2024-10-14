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
    public class MentorSkillBusinessEntity
    {
        private IUnitOfWork work;
        public MentorSkillBusinessEntity(IUnitOfWork work)
        {
            this.work = work;
        }

        public async Task<IEnumerable<MentorSkill>> GetMentorSkillsAsync()
        {
            return await work.MentorSkills.GetAllAsync();
        }

        public async Task<MentorSkill> GetMentorSkillAsync(string id)
        {
            return (await work.MentorSkills.GetAllAsync())
                .Where(m => m.Id.Equals(id))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<MentorSkill>> GetMentorSkillsByMentorIdAsync(string mentorId)
        {
            List<MentorSkill> mentorSkills = (await work.MentorSkills.GetAllAsync())
                .Where(m => m.MentorId.Equals(mentorId)).ToList();
            foreach(var m in mentorSkills)
            {
                m.Skill = await work.Skills.GetAsync(m.SkillId);
            }
            return mentorSkills;
        }

        public async Task<MentorSkill> AddMentorSkills(MentorSkill mentorSkills)
        {
            string id = HmsUtils.CreateGuid();
            mentorSkills.Id = id;
            await work.MentorSkills.AddAsync(mentorSkills);
            await work.Save();
            return mentorSkills;
        }

        public async Task<MentorSkill> UpdateMentorSkills(MentorSkill mentorSkills)
        {
            work.MentorSkills.Update(mentorSkills);
            await work.Save();
            return mentorSkills;
        }

        public async Task RemoveMentorSkills(string id)
        {
            MentorSkill mentorSkill = await work.MentorSkills.GetAsync(id);
            work.MentorSkills.Delete(mentorSkill);
            await work.Save();
        }

        public async Task RemoveMentorSkillsBySkillId(string SkillId)
        {
            IEnumerable<MentorSkill> mentorSkills = (await work.MentorSkills.GetAllAsync())
                .Where(ms => ms.SkillId == SkillId);
            foreach(MentorSkill ms in mentorSkills)
            {
                work.MentorSkills.Delete(ms);
            }
            await work.Save();
        }
    }
}
