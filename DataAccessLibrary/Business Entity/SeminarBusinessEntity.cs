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
    public class SeminarBusinessEntity
    {
        private IUnitOfWork work; 
        public SeminarBusinessEntity(IUnitOfWork work)
        {
            this.work = work;
        }
        public async Task<IEnumerable<Seminar>> GetSeminarsAsync()
        {
            return (await work.Seminars.GetAllAsync())
                .Where(s => !s.IsDeleted);
        }

        public async Task<Seminar> GetSeminarAsync(string id)
        {
            return (await work.Seminars.GetAllAsync())
                .Where(s => !s.IsDeleted && s.Id.Equals(id))
                .FirstOrDefault();
        }

        public async Task<Seminar> AddSeminar(Seminar seminar)
        {
            string id = HmsUtils.CreateGuid();
            seminar.Id = id;
            seminar.IsDeleted = false;
            seminar.CreatedDate = DateTime.Now;
            //var instance = new MentorDetailsBusinessEntity(work);
            seminar.Mentor = await work.MentorDetails.GetAsync(seminar.MentorId);
            User mentor = await work.Users.GetAsync(seminar.MentorId);
            SeminarParticipant p = new SeminarParticipant();
            p.SeminarId = id;
            p.Seminar = seminar;
            p.UserId = seminar.MentorId;
            p.User = mentor;
            seminar.SeminarParticipants
                .Add(p);
            await work.Seminars.AddAsync(seminar);
            await work.Save();
            return seminar;
        }

        public async Task<Seminar> UpdateSeminar(Seminar seminar)
        {
            work.Seminars.Update(seminar);
            await work.Save();
            return seminar;
        }

        public async Task RemoveSeminar(string id)
        {
            Seminar seminar = await work.Seminars.GetAsync(id);
            seminar.IsDeleted = true;
            work.Seminars.Update(seminar);
            await work.Save();
        }


        //When admin deactivate a mentor account --> All mentor's future seminars will be set as deleted
        public async Task ReleaseSeminar(string MentorId)
        {
            IEnumerable<Seminar> seminars = (await work.Seminars.GetAllAsync())
                .Where(s => !s.IsDeleted && s.MentorId == MentorId && DateTime.Now.CompareTo(s.StartDate) <= 0);
            foreach (Seminar s in seminars)
            {
                s.IsDeleted = true;
                work.Seminars.Update(s);
            }
            await work.Save();
        }

        public async Task<MenteeSeminarDTO> GetMenteeSeminarsAsync(string menteeId)
        {
            User user = await work.Users.GetAsync(menteeId);
            if (user.Role != UserRole.MENTEE)
            {
                throw new ApplicationException("User role is not allowed to do this operation!");
            }

            MenteeSeminarDTO menteeSeminar = new MenteeSeminarDTO();

            IEnumerable<Seminar> seminars = (await work.Seminars.GetAllAsync(nameof(Seminar.SeminarParticipants), nameof(Seminar.Mentor)))
                .Where(s => !s.IsDeleted);

            foreach (Seminar seminar in seminars)
            {
                User mentor = await work.Users.GetAsync(seminar.MentorId);
                seminar.Mentor.Mentor = mentor;
            }

            menteeSeminar.MenteeSeminars = seminars
                .Where(s => s.SeminarParticipants.Any(participant => participant.UserId.Equals(menteeId)))
                .OrderByDescending(s => s.StartDate);

            menteeSeminar.UpcomingSeminars = seminars
                .Where(s =>
                !s.SeminarParticipants.Any(participant => participant.UserId.Equals(menteeId)) &&
                DateTime.Now.CompareTo(s.RegistrationDeadline) <= 0)
                .OrderByDescending(s => s.StartDate);

            return menteeSeminar;
        }

        public async Task<MentorSeminarDTO> GetMentorSeminarsAsync(string mentorId)
        {
            User user = await work.Users.GetAsync(mentorId);
            if (user.Role != UserRole.MENTOR)
            {
                throw new ApplicationException("User role is not allowed to do this operation!");
            }

            MentorSeminarDTO mentorSeminar = new MentorSeminarDTO();

            IEnumerable<Seminar> seminars = (await work.Seminars.GetAllAsync(nameof(Seminar.SeminarParticipants), nameof(Seminar.Mentor)))
                .Where(s => !s.IsDeleted && s.MentorId.Equals(mentorId));

            foreach (Seminar seminar in seminars)
            {
                User mentor = await work.Users.GetAsync(seminar.MentorId);
                    seminar.Mentor.Mentor = mentor;
            }

            mentorSeminar.MentorSeminars = seminars
                .Where(s => s.SeminarParticipants.Any(participant => participant.UserId.Equals(mentorId)))
                .OrderByDescending(s => s.StartDate);

            mentorSeminar.UpcomingSeminars = seminars
                .Where(s =>
                !s.SeminarParticipants.Any(participant => participant.UserId.Equals(mentorId)) &&
                DateTime.Now.CompareTo(s.RegistrationDeadline) <= 0)
                .OrderByDescending(s => s.StartDate);

            //IEnumerable<Seminar> mentorSeminar = (await work.Seminars.GetAllAsync()).Where(m => m.MentorId == mentorId).ToList();


            return mentorSeminar;
        }

        public async Task<MentorOtherSeminarDTO> GetMentorOtherSeminarsAsync(string mentorId)
        {
            User user = await work.Users.GetAsync(mentorId);
            if (user.Role != UserRole.MENTOR)
            {
                throw new ApplicationException("User role is not allowed to do this operation!");
            }

            MentorOtherSeminarDTO mentorSeminar = new MentorOtherSeminarDTO();

            IEnumerable<Seminar> seminars = (await work.Seminars.GetAllAsync(nameof(Seminar.SeminarParticipants), nameof(Seminar.Mentor)))
                .Where(s => !s.IsDeleted && !s.MentorId.Equals(mentorId));

            foreach (Seminar seminar in seminars)
            {
                User mentor = await work.Users.GetAsync(seminar.MentorId);
                seminar.Mentor.Mentor = mentor;
            }

            mentorSeminar.MentorSeminars = seminars
                .Where(s => s.SeminarParticipants.Any(participant => participant.UserId.Equals(mentorId)))
                .OrderByDescending(s => s.StartDate);

            mentorSeminar.UpcomingSeminars = seminars
                .Where(s =>
                !s.SeminarParticipants.Any(participant => participant.UserId.Equals(mentorId)) &&
                DateTime.Now.CompareTo(s.RegistrationDeadline) <= 0)
                .OrderByDescending(s => s.StartDate);

            return mentorSeminar;
        }

    }
}
