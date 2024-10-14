using BusinessObjectLibrary;
using DataAccessLibrary.Interfaces;
using DTOsLibrary;
using HmsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataAccessLibrary.Business_Entity
{
    public class SkillBusinessEntity
    {
        private IUnitOfWork work;
        public SkillBusinessEntity(IUnitOfWork work)
        {
            this.work = work;
        }

        public async Task<IEnumerable<Skill>> GetSkillsAsync()
        {
            return (await work.Skills.GetAllAsync())
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Name);
        }

        public async Task<Skill> GetSkillAsync(string id)
        {
            return (await work.Skills.GetAllAsync())
                .Where(s => !s.IsDeleted && s.Id.Equals(id))
                .FirstOrDefault();
        }

        public async Task<Skill> AddSkill(Skill skill)
        {
            string id = HmsUtils.CreateGuid();
            skill.Id = id;
            skill.IsDeleted = false;
            skill.CreatedDate = DateTime.Now;
            await work.Skills.AddAsync(skill);
            await work.Save();
            return skill;
        }

        public async Task<Skill> UpdateSkill(Skill skill)
        {
            work.Skills.Update(skill);
            await work.Save();
            return skill;
        }

        public async Task RemoveSkill(string id)
        {
            Skill skill = await work.Skills.GetAsync(id);
            skill.IsDeleted = true;
            work.Skills.Update(skill);

            //Invalidate floating request with removed skill related
            IEnumerable<Request> floatingReqs = (await work.Requests.GetAllAsync(nameof(Request.RequestSkills)))
                .Where(r => !r.IsDeleted && r.MentorId == null && r.RequestSkills.Count() == 1
                && r.RequestSkills.FirstOrDefault().RequestId == id);
            foreach(var req in floatingReqs)
            {
                req.Status = RequestStatus.INVALID;
                req.Content += "ADMIN_COMMENT: Sorry, the selected skill has been removed! Please create another request";
            }
            await work.Save();
        }

        public async Task<List<SkillDTO>> GetSkillDTO()
        {
            List<Skill> skills = (await work.Skills.GetAllAsync()).Where(s => !s.IsDeleted).ToList();
            List<RequestSkill> requestSkills = (await work.RequestSkills.GetAllAsync()).ToList();
            List<SkillDTO> result = new List<SkillDTO>();
            var skillDTOs = from s in requestSkills
                        group s by s.SkillId into c
                        let count = c.Count()
                        orderby count descending
                        select new { SkillId = c.Key, Count = count };
            foreach(var skill in skillDTOs)
            {
                result.Add(new SkillDTO
                {
                    Id = skill.SkillId,
                    RequestedTime = skill.Count,
                    Name = skills.Where(s => s.Id == skill.SkillId).FirstOrDefault().Name
                });
            }
            return result;
        }
    }
}
