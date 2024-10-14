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
    public class SeminarParticipantBusinessEntity
    {
        private IUnitOfWork work;
        public SeminarParticipantBusinessEntity(IUnitOfWork work)
        {
            this.work = work;
        }

        public async Task<IEnumerable<SeminarParticipant>> GetSeminarParticipantsAsync()
        {
            return await work.SeminarParticipants.GetAllAsync();
        }

        public async Task<SeminarParticipant> AddSeminarParticipant(SeminarParticipant seminarParticipant)
        {
            User user = await work.Users.GetAsync(seminarParticipant.UserId);
            if (user == null)
            {
                throw new ApplicationException("User does not exist!!");
            }
            Seminar seminar = await work.Seminars.GetAsync(seminarParticipant.SeminarId);
            if (seminar == null)
            {
                throw new ApplicationException("Seminar does not exist!!");
            }

            if (DateTime.Now > seminar.RegistrationDeadline)
            {
                throw new ApplicationException("It's too late to register to join this seminar!!");
            }

            IEnumerable<Seminar> registeredSeminars = (await work.SeminarParticipants.GetAllAsync(nameof(Seminar)))
                .Where(sp => sp.UserId.Equals(seminarParticipant.UserId))
                .Select(sp => sp.Seminar);

            if (user.Role == UserRole.MENTOR)
            {
                registeredSeminars = registeredSeminars.Concat((await work.Seminars.GetAllAsync())
                    .Where(s => s.MentorId.Equals(seminarParticipant.UserId)));
            }

            foreach (Seminar registeredSeminar in registeredSeminars)
            {
                if (seminar.StartDate.IsDateBetween(registeredSeminar.StartDate, registeredSeminar.EndDate)
                    || seminar.EndDate.IsDateBetween(registeredSeminar.StartDate, registeredSeminar.EndDate))
                {
                    throw new ApplicationException($"The Date of this seminar is not sufficient with one of your seminars. " +
                        $"Seminar Details: {registeredSeminar.Topic}. Start Date: {registeredSeminar.StartDate}, End Date: {registeredSeminar.EndDate}");
                }
            }

            await work.SeminarParticipants.AddAsync(seminarParticipant);
            await work.Save();
            return seminarParticipant;
        }

        public async Task<SeminarParticipant> UpdateSeminarParticipant(SeminarParticipant seminarParticipant)
        {
            work.SeminarParticipants.Update(seminarParticipant);
            await work.Save();
            return seminarParticipant;
        }

        public async Task RemoveSeminarParticipant(string seminarId, string menteeId)
        {
            SeminarParticipant seminarParticipant = await work.SeminarParticipants.GetAsync(seminarId, menteeId);
            if (seminarParticipant == null)
            {
                throw new ApplicationException("Mentee have not registered to join the seminar!!");
            }

            work.SeminarParticipants.Delete(seminarParticipant);
            await work.Save();
        }
    }
}
