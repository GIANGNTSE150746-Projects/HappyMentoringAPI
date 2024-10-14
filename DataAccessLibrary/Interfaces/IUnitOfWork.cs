using BusinessObjectLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        #region Repositories
        IGenericRepository<Cv> Cvs { get; }
        IGenericRepository<MentorSkill> MentorSkills { get; }
        IGenericRepository<MentorDetail> MentorDetails { get; }
        IGenericRepository<Rating> Ratings { get; }
        IGenericRepository<Request> Requests { get; }
        IGenericRepository<RequestSkill> RequestSkills { get; }
        IGenericRepository<Seminar> Seminars { get; }
        IGenericRepository<SeminarParticipant> SeminarParticipants { get; }
        IGenericRepository<Skill> Skills { get; }
        IGenericRepository<TeachingThread> TeachingThreads { get; }
        IGenericRepository<User> Users { get; }
        #endregion

        /// <summary>
        /// Save changes to database
        /// </summary>
        /// <returns></returns>
        Task<int> Save();
    }
}
