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
    public class TeachingThreadBusinessEntity
    {
        private IUnitOfWork work;
        public TeachingThreadBusinessEntity(IUnitOfWork work)
        {
            this.work = work;
        }

        public async Task<TeachingThread> GetTeachingThreadByRequestId(string requestId)
        {
            return (await work.TeachingThreads.GetAllAsync()).Where(tr => tr.RequestId == requestId && !tr.IsDeleted).FirstOrDefault();
        }
        public async Task<IEnumerable<TeachingThread>> GetTeachingThreadsAsync()
        {
            return (await work.TeachingThreads.GetAllAsync())
                .Where(tr => !tr.IsDeleted);
        }

        public async Task<TeachingThread> GetTeachingThreadAsync(string id)
        {
            return (await work.TeachingThreads.GetAllAsync())
                .Where(tr => !tr.IsDeleted && tr.Id.Equals(id))
                .FirstOrDefault();
        }

        public async Task<TeachingThread> AddTeachingThread(TeachingThread teachingThread)
        {
            string id = HmsUtils.CreateGuid();
            teachingThread.Id = id;
            teachingThread.IsDeleted = false;
            teachingThread.CreatedDate = DateTime.Now;
            teachingThread.Request = null;
            await work.TeachingThreads.AddAsync(teachingThread);
            await work.Save();
            return teachingThread;
        }

        public async Task<TeachingThread> UpdateTeachingThread(TeachingThread teachingThread)
        {
            work.TeachingThreads.Update(teachingThread);
            await work.Save();
            return teachingThread;
        }

        public async Task RemoveTeachingThread(string id)
        {
            TeachingThread teachingThread = await work.TeachingThreads.GetAsync(id);
            teachingThread.IsDeleted = true;
            work.TeachingThreads.Update(teachingThread);
            await work.Save();
        }
    }
}
