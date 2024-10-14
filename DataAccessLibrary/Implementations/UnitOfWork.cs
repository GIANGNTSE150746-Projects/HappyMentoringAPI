using BusinessObjectLibrary;
using DataAccessLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HmsContext context;

        public IGenericRepository<Cv> Cvs { get; }

        public IGenericRepository<MentorSkill> MentorSkills { get; }

        public IGenericRepository<Rating> Ratings { get; }

        public IGenericRepository<Request> Requests { get; }

        public IGenericRepository<RequestSkill> RequestSkills { get; }

        public IGenericRepository<Seminar> Seminars { get; }

        public IGenericRepository<SeminarParticipant> SeminarParticipants { get; }

        public IGenericRepository<Skill> Skills { get; }

        public IGenericRepository<TeachingThread> TeachingThreads { get; }

        public IGenericRepository<User> Users { get; }

        public IGenericRepository<MentorDetail> MentorDetails { get; }

        public UnitOfWork(HmsContext context,
            IGenericRepository<Cv> cvs,
            IGenericRepository<MentorSkill> mentorSkills,
            IGenericRepository<Rating> ratings,
            IGenericRepository<Request> requests,
            IGenericRepository<RequestSkill> requestSkills,
            IGenericRepository<Seminar> seminars,
            IGenericRepository<SeminarParticipant> seminarParticipants,
            IGenericRepository<Skill> skills,
            IGenericRepository<TeachingThread> teachingThreads,
            IGenericRepository<User> users,
            IGenericRepository<MentorDetail> metorDetails)
        {
            this.context = context;
            Cvs = cvs;
            MentorSkills = mentorSkills;
            Ratings = ratings;
            Requests = requests;
            RequestSkills = requestSkills;
            Seminars = seminars;
            SeminarParticipants = seminarParticipants;
            Skills = skills;
            TeachingThreads = teachingThreads;
            Users = users;
            MentorDetails = metorDetails;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }

        public async Task<int> Save()
        {
            return await context.SaveChangesAsync();
        }
    }
}
