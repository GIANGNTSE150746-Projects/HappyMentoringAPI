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
    public class RequestBusinessEntity
    {
        private IUnitOfWork work;
        public RequestBusinessEntity(IUnitOfWork work)
        {
            this.work = work;
        }

        public async Task<IEnumerable<Request>> GetFloatingRequest(string mentorId)
        {
            IList<MentorSkill> mentorSkill = (await work.MentorSkills.GetAllAsync()).Where(ms =>
            ms.MentorId == mentorId)
                .ToList();
            IList<RequestSkill> requestSkill = (await work.RequestSkills.GetAllAsync()).Where(rs =>
            mentorSkill.Any(ms => ms.SkillId == rs.SkillId))
                .ToList();
            var list = (await work.Requests.GetAllAsync(nameof(Request.RequestSkills)))
                .Where(r => !r.IsDeleted && string.IsNullOrEmpty(r.MentorId) && requestSkill.Any(rs => rs.RequestId == r.Id));
            foreach (Request request in list)
            {
                foreach (RequestSkill rSkill in request.RequestSkills)
                {
                    rSkill.Skill = await work.Skills.GetAsync(rSkill.SkillId);
                    rSkill.Request = null;
                }
            }
            return list;
        }

        public async Task<IEnumerable<Request>> GetRequestsForMentorAsync(string mentorId)
        {
            var list = (await work.Requests.GetAllAsync(nameof(Request.RequestSkills)))
                .Where(r => !r.IsDeleted && r.MentorId == mentorId);
            foreach (Request request in list)
            {
                foreach (RequestSkill rSkill in request.RequestSkills)
                {
                    rSkill.Skill = await work.Skills.GetAsync(rSkill.SkillId);
                    rSkill.Request = null;
                }
            }
            return list;
        }

        public async Task<IEnumerable<Request>> GetMenteeRequests(string menteeId)
        {
            IEnumerable<Request> requests = (await work.Requests.GetAllAsync(nameof(Request.RequestSkills), nameof(Request.Mentor)))
                .Where(req => req.IsDeleted == false && req.MenteeId.Equals(menteeId))
                .OrderBy(req => req.Status)
                .ThenByDescending(req => req.CreatedDate);

            foreach (Request request in requests)
            {
                foreach (RequestSkill requestSkill in request.RequestSkills)
                {
                    requestSkill.Skill = await work.Skills.GetAsync(requestSkill.SkillId);
                    requestSkill.Request = null;
                }

                if (request.Mentor != null)
                {
                    request.Mentor.Mentor = await work.Users.GetAsync(request.MentorId);

                }
            }


            return requests;
        }

        public async Task<IEnumerable<Request>> GetConcludedRequestsAsync()
        {
            return (await work.Requests.GetAllAsync(nameof(Request.Mentee)))
                .Where(r => !r.IsDeleted &&  r.Status == RequestStatus.CONCLUDED);
        }

        public async Task<IEnumerable<Request>> GetRequestsAsync()
        {
            IEnumerable<Request> requests = (await work.Requests.GetAllAsync(nameof(Request.RequestSkills), nameof(Request.Mentor)))
                .Where(req => !req.IsDeleted).OrderBy(req => req.Status);

            foreach (Request request in requests)
            {
                foreach (RequestSkill requestSkill in request.RequestSkills)
                {
                    requestSkill.Skill = await work.Skills.GetAsync(requestSkill.SkillId);
                    requestSkill.Request = null;
                }

                if (request.Mentor != null)
                {
                    request.Mentor.Mentor = await work.Users.GetAsync(request.MentorId);

                }
            }

            return requests;
        }

        public async Task<List<ReportedRequestDTO>> GetReportedRequestsAsync()
        {
            List<ReportedRequestDTO> requests = (await work.Requests.GetAllAsync())
                .Where(r => !r.IsDeleted && r.Status == RequestStatus.REPORTED)
                .Join(
                    await work.Users.GetAllAsync(),
                    r => r.MenteeId,
                    u => u.Id,
                    (r, u) => new 
                    {
                        Id = r.Id,
                        Topic = r.Topic,
                        Content = r.Content,
                        MenteeName = u.Name,
                        MentorId = r.MentorId
                    }
                 ).ToList()
                .Join(
                    await work.Users.GetAllAsync(),
                    r => r.MentorId,
                    u => u.Id,
                    (r, u) => new ReportedRequestDTO
                    {
                        RequestId = r.Id,
                        Topic = r.Topic,
                        Content = r.Content,
                        MenteeName = r.MenteeName,
                        MentorName = u.Name,
                        Status = RequestStatus.REPORTED
                    }
                 ).ToList();
            return requests;
        }

        public async Task<Request> UnflagReportedRequest(ReportedRequestDTO reportedRequest)
        {
            Request request = await work.Requests.GetAsync(reportedRequest.RequestId);
            request.Content = reportedRequest.Content;
            request.Status = reportedRequest.Status;
            work.Requests.Update(request);
            await work.Save();
            return request;
        }

        public async Task<Request> GetRequestAsync(string id)
        {
            return (await work.Requests.GetAllAsync())
                .Where(r => !r.IsDeleted && r.Id.Equals(id))
                .FirstOrDefault();
        }

        public async Task<Request> AddRequest(Request request)
        {
            await CheckRequest(request);
            string id = HmsUtils.CreateGuid();
            request.Id = id;
            request.IsDeleted = false;
            request.CreatedDate = DateTime.Now;
            await work.Requests.AddAsync(request);
            await work.Save();
            return request;
        }

        public async Task<Request> UpdateRequest(Request request)
        {
            request.Mentor = null;
            await CheckRequest(request);

            IEnumerable<RequestSkill> requestSkills = (await work.RequestSkills.GetAllAsync())
                .Where(rs => rs.RequestId.Equals(request.Id));
            foreach (RequestSkill requestSkill in requestSkills)
            {
                work.RequestSkills.Delete(requestSkill);
            }
            foreach (var requestSkill in request.RequestSkills)
            {
               requestSkill.Skill = null;
            }

            var tmpRequest = request;
            work.Requests.Update(request);
            await work.Save();
            return request;
        }

        private async Task RemoveRequestSkill(Request request)
        {
            IEnumerable<RequestSkill> requestSkills = (await work.RequestSkills.GetAllAsync())
                .Where(rs => rs.RequestId.Equals(request.Id));
            foreach (RequestSkill requestSkill in requestSkills)
            {
                work.RequestSkills.Delete(requestSkill);
            }
            await work.Save();
        }

        //When admin deactivate a mentor account --> All mentor's pending requests will be set as MentorId to null
        public async Task ReleaseRequest(string MentorId)
        {
            IEnumerable<Request> requests = (await work.Requests.GetAllAsync())
                .Where(r => !r.IsDeleted && r.MentorId == MentorId && r.Status == RequestStatus.PENDING);
            foreach(Request r in requests)
            {
                r.MentorId = null;
                work.Requests.Update(r);
            }
            await work.Save();
        }

        public async Task RemoveRequest(string id)
        {
            Request request = await work.Requests.GetAsync(id);
            request.IsDeleted = true;
            work.Requests.Update(request);
            await work.Save();
        }

        private async Task CheckRequest(Request request)
        {
            if ((await work.Users.GetAllAsync()).FirstOrDefault(
                user => !user.IsDeleted && user.Role == UserRole.MENTEE && user.Id.Equals(request.MenteeId)) == null)
            {
                throw new ApplicationException("Mentee with the ID " + request.MenteeId + " does not exist!!");
            }

            if (!string.IsNullOrEmpty(request.MentorId) && (await work.Users.GetAllAsync()).FirstOrDefault(
                user => !user.IsDeleted && user.Role == UserRole.MENTOR && user.Id.Equals(request.MentorId)) == null)
            {
                throw new ApplicationException("Mentor with the ID " + request.MentorId + " does not exist!!");
            }
            var r = (await work.Requests.GetAllAsync(false)).FirstOrDefault(o => o.Id == request.Id);
            if(r != null && r.MentorId != null && r.MentorId != request.MentorId)
            {
                throw new ApplicationException("This request is no longer available!");
            }

            if (request.RequestSkills == null || !request.RequestSkills.Any())
            {
                throw new ApplicationException("A request needs to have at least one skill!!");
            }
            foreach (RequestSkill requestSkill in request.RequestSkills)
            {
                if ((await work.Skills.GetAsync(requestSkill.SkillId)) == null)
                {
                    throw new ApplicationException("Skill with the ID " + requestSkill.SkillId + " does not exist!!");
                }
            }
        }
        //Admin delete mentee --> pending request of this mentee will be removed.
        public async Task DeleteRequestByMenteeId(string MenteeId)
        {
            IEnumerable<Request> requests = (await work.Requests.GetAllAsync())
                .Where(r => !r.IsDeleted && r.MenteeId == MenteeId && r.Status == RequestStatus.PENDING);
            foreach(Request r in requests)
            {
                r.IsDeleted = true;
                work.Requests.Update(r);
            }
            await work.Save();
        }

        public async Task<AdminReportDTO> GetAdminReportAsync(string filter)
        {
            AdminReportDTO adminReport = new AdminReportDTO();

            #region Top
            //Mentor, Mentee
            List<User> currentMonthUsers = (await work.Users.GetAllAsync())
               .Where(u => !u.IsDeleted
                   && u.CreatedDate.Month == DateTime.Now.Month).ToList();

            List<User> lastMonthUsers = (await work.Users.GetAllAsync())
                .Where(u => !u.IsDeleted
                    && u.CreatedDate.Month == (DateTime.Now.Month > 1 ? DateTime.Now.Month - 1 : 1)).ToList();

            adminReport.MentorCount = currentMonthUsers.Where(u => u.Role == UserRole.MENTOR).Count();

            int currentMentorCount = adminReport.MentorCount;
            int lastMonthMentorCount = lastMonthUsers.Where(u => u.Role == UserRole.MENTOR).Count();
            adminReport.MentorPercentage = (currentMentorCount == 0 || lastMonthMentorCount == 0) ?
                (currentMentorCount >= lastMonthMentorCount ? (currentMentorCount == lastMonthMentorCount ? 0 : 100) : 0)
                :
                (currentMentorCount >= lastMonthMentorCount ?
                    (currentMentorCount == lastMonthMentorCount ? 0 : (float)lastMonthMentorCount / currentMentorCount * 100)
                    : -((float)currentMentorCount / lastMonthMentorCount * 100)
                );
            adminReport.MenteeCount = currentMonthUsers.Where(u => u.Role == UserRole.MENTEE).Count();

            int currentMenteeCount = adminReport.MenteeCount;
            int lastMonthMenteeCount = lastMonthUsers.Where(u => u.Role == UserRole.MENTEE).Count();
            adminReport.MenteePercentage = (currentMenteeCount == 0 || lastMonthMenteeCount == 0) ?
                (currentMenteeCount >= lastMonthMenteeCount ? (currentMenteeCount == lastMonthMenteeCount ? 0 : 100) : 0)
                :
                (currentMenteeCount >= lastMonthMenteeCount ?
                    (currentMenteeCount == lastMonthMenteeCount ? 0 : (float)lastMonthMenteeCount / currentMenteeCount * 100)
                    : -((float)currentMenteeCount / lastMonthMenteeCount * 100)
                );

            //Request
            int currentRequestCount = (await work.Requests.GetAllAsync())
                .Where(r => !r.IsDeleted
                    && r.CreatedDate.Month == DateTime.Now.Month).ToList().Count();

            int lastMonthRequestCount = (await work.Requests.GetAllAsync())
                .Where(r => !r.IsDeleted
                    && r.CreatedDate.Month == (DateTime.Now.Month > 1 ? DateTime.Now.Month - 1 : 1)).ToList().Count();

            adminReport.RequestCount = currentRequestCount;
            adminReport.RequestPercentage = (currentRequestCount == 0 || lastMonthRequestCount == 0) ?
                (currentRequestCount >= lastMonthRequestCount ? (currentRequestCount == lastMonthRequestCount ? 0 : 100) : 0)
                :
                (currentRequestCount >= lastMonthRequestCount ?
                    (currentRequestCount == lastMonthRequestCount ? 0 : (float)lastMonthRequestCount / currentRequestCount * 100)
                    : -((float)currentRequestCount / lastMonthRequestCount * 100)
                );

            //Seminar
            int currentSeminarCount = (await work.Seminars.GetAllAsync())
                .Where(s => !s.IsDeleted
                    && s.CreatedDate.Month == DateTime.Now.Month).ToList().Count();

            int lastMonthSeminarCount = (await work.Seminars.GetAllAsync())
                .Where(s => !s.IsDeleted
                    && s.CreatedDate.Month == (DateTime.Now.Month > 1 ? DateTime.Now.Month - 1 : 1)).ToList().Count();

            adminReport.SeminarCount = currentSeminarCount;
            adminReport.SeminarPercentage = (currentSeminarCount == 0 || lastMonthSeminarCount == 0) ?
                (currentSeminarCount >= lastMonthSeminarCount ? (currentSeminarCount == lastMonthSeminarCount ? 0 : 100) : 0)
                :
                (currentSeminarCount >= lastMonthSeminarCount ?
                    (currentSeminarCount == lastMonthSeminarCount ? 0 : (float)lastMonthSeminarCount / currentSeminarCount * 100)
                    : -((float)currentSeminarCount / lastMonthSeminarCount * 100)
                );
            #endregion

            #region Main
            if (filter.Equals("week"))
            {
                List<DateTime> days = HmsUtils.GetDaysOfCurrentMonth();
                List<DateTime> weeks = new List<DateTime>();
                for (int i = 0; i < days.Count(); i += 6)
                {
                    weeks.Add(days[i]);
                }

                List<User> mentors = (await work.Users.GetAllAsync())
                    .Where(u => !u.IsDeleted && u.CreatedDate.Month == DateTime.Now.Month && u.Role == UserRole.MENTOR).ToList();
                List<User> mentees = (await work.Users.GetAllAsync())
                    .Where(u => !u.IsDeleted && u.CreatedDate.Month == DateTime.Now.Month && u.Role == UserRole.MENTEE).ToList();
                List<Request> requests = (await work.Requests.GetAllAsync())
                    .Where(r => !r.IsDeleted && r.CreatedDate.Month == DateTime.Now.Month).ToList();
                List<Seminar> seminars = (await work.Seminars.GetAllAsync())
                    .Where(s => !s.IsDeleted && s.CreatedDate.Month == DateTime.Now.Month).ToList();

                //Requests
                int week1RequestCount = requests
                    .Where(r => r.CreatedDate.CompareTo(weeks[0]) >= 0 && r.CreatedDate.CompareTo(weeks[0].AddDays(6)) <= 0).Count();
                int week2RequestCount = requests
                    .Where(r => r.CreatedDate.CompareTo(weeks[1]) >= 0 && r.CreatedDate.CompareTo(weeks[1].AddDays(6)) <= 0).Count();
                int week3RequestCount = requests
                    .Where(r => r.CreatedDate.CompareTo(weeks[2]) >= 0 && r.CreatedDate.CompareTo(weeks[2].AddDays(6)) <= 0).Count();
                int week4RequestCount = requests
                    .Where(r => r.CreatedDate.CompareTo(weeks[3]) >= 0 && r.CreatedDate.CompareTo(weeks[3].AddDays(days.Count() - 21)) <= 0).Count();

                adminReport.RequestWeekCount = new List<int>();
                adminReport.RequestWeekCount.Add(week1RequestCount);
                adminReport.RequestWeekCount.Add(week2RequestCount);
                adminReport.RequestWeekCount.Add(week3RequestCount);
                adminReport.RequestWeekCount.Add(week4RequestCount);

                //Mentors
                int week1MentorCount = mentors
                    .Where(r => r.CreatedDate.CompareTo(weeks[0]) >= 0 && r.CreatedDate.CompareTo(weeks[0].AddDays(6)) <= 0).Count();
                int week2MentorCount = mentors
                    .Where(r => r.CreatedDate.CompareTo(weeks[1]) >= 0 && r.CreatedDate.CompareTo(weeks[1].AddDays(6)) <= 0).Count();
                int week3MentorCount = mentors
                    .Where(r => r.CreatedDate.CompareTo(weeks[2]) >= 0 && r.CreatedDate.CompareTo(weeks[2].AddDays(6)) <= 0).Count();
                int week4MentorCount = mentors
                    .Where(r => r.CreatedDate.CompareTo(weeks[3]) >= 0 && r.CreatedDate.CompareTo(weeks[3].AddDays(days.Count() - 21)) <= 0).Count();

                adminReport.MentorWeekCount = new List<int>();
                adminReport.MentorWeekCount.Add(week1MentorCount);
                adminReport.MentorWeekCount.Add(week2MentorCount);
                adminReport.MentorWeekCount.Add(week3MentorCount);
                adminReport.MentorWeekCount.Add(week4MentorCount);

                //Mentees
                int week1MenteeCount = mentees
                    .Where(r => r.CreatedDate.CompareTo(weeks[0]) >= 0 && r.CreatedDate.CompareTo(weeks[0].AddDays(6)) <= 0).Count();
                int week2MenteeCount = mentees
                    .Where(r => r.CreatedDate.CompareTo(weeks[1]) >= 0 && r.CreatedDate.CompareTo(weeks[1].AddDays(6)) <= 0).Count();
                int week3MenteeCount = mentees
                    .Where(r => r.CreatedDate.CompareTo(weeks[2]) >= 0 && r.CreatedDate.CompareTo(weeks[2].AddDays(6)) <= 0).Count();
                int week4MenteeCount = mentees
                    .Where(r => r.CreatedDate.CompareTo(weeks[3]) >= 0 && r.CreatedDate.CompareTo(weeks[3].AddDays(days.Count() - 21)) <= 0).Count();

                adminReport.MenteeWeekCount = new List<int>();
                adminReport.MenteeWeekCount.Add(week1MenteeCount);
                adminReport.MenteeWeekCount.Add(week2MenteeCount);
                adminReport.MenteeWeekCount.Add(week3MenteeCount);
                adminReport.MenteeWeekCount.Add(week4MenteeCount);

                //Seminars
                int week1SeminarCount = seminars
                    .Where(r => r.CreatedDate.CompareTo(weeks[0]) >= 0 && r.CreatedDate.CompareTo(weeks[0].AddDays(6)) <= 0).Count();
                int week2SeminarCount = seminars
                    .Where(r => r.CreatedDate.CompareTo(weeks[1]) >= 0 && r.CreatedDate.CompareTo(weeks[1].AddDays(6)) <= 0).Count();
                int week3SeminarCount = seminars
                    .Where(r => r.CreatedDate.CompareTo(weeks[2]) >= 0 && r.CreatedDate.CompareTo(weeks[2].AddDays(6)) <= 0).Count();
                int week4SeminarCount = seminars
                    .Where(r => r.CreatedDate.CompareTo(weeks[3]) >= 0 && r.CreatedDate.CompareTo(weeks[3].AddDays(days.Count() - 21)) <= 0).Count();

                adminReport.SeminarWeekCount = new List<int>();
                adminReport.SeminarWeekCount.Add(week1SeminarCount);
                adminReport.SeminarWeekCount.Add(week2SeminarCount);
                adminReport.SeminarWeekCount.Add(week3SeminarCount);
                adminReport.SeminarWeekCount.Add(week4SeminarCount);
            }
            else if (filter.Equals("month"))
            {
                List<User> users = (await work.Users.GetAllAsync())
                       .Where(a => !a.IsDeleted && a.CreatedDate.Year == DateTime.Now.Year).ToList();

                //Mentors
                List<User> mentors = users.Where(a => a.Role == UserRole.MENTOR).ToList();
                adminReport.MentorMonthCount = new List<int>();
                for (int i = 1; i <= 12; i++)
                {
                    adminReport.MentorMonthCount.Add(mentors.Where(a => a.CreatedDate.Month == i).Count());
                }

                //Mentees
                List<User> mentees = users.Where(a => a.Role == UserRole.MENTEE).ToList();
                adminReport.MenteeMonthCount = new List<int>();
                for (int i = 1; i <= 12; i++)
                {
                    adminReport.MenteeMonthCount.Add(mentees.Where(a => a.CreatedDate.Month == i).Count());
                }

                //Request
                List<Request> requests = (await work.Requests.GetAllAsync())
                       .Where(a => !a.IsDeleted && a.CreatedDate.Year == DateTime.Now.Year).ToList();
                adminReport.RequestMonthCount = new List<int>();
                for (int i = 1; i <= 12; i++)
                {
                    adminReport.RequestMonthCount.Add(requests.Where(a => a.CreatedDate.Month == i).Count());
                }

                //Seminar
                List<Seminar> seminars = (await work.Seminars.GetAllAsync())
                       .Where(a => !a.IsDeleted && a.CreatedDate.Year == DateTime.Now.Year).ToList();
                adminReport.SeminarMonthCount = new List<int>();
                for (int i = 1; i <= 12; i++)
                {
                    adminReport.SeminarMonthCount.Add(seminars.Where(a => a.CreatedDate.Month == i).Count());
                }
            }
            else
            {
                IEnumerable<User> users = (await work.Users.GetAllAsync()).Where(a => !a.IsDeleted);

                //Mentors
                adminReport.MentorThreeYearCount = new List<int>();
                for (int i = 2; i >= 0; i--)
                {
                    adminReport.MentorThreeYearCount
                        .Add(users.Where(a => a.Role == UserRole.MENTOR && a.CreatedDate.Year == DateTime.Now.Year - i).Count());
                }

                //Mentees
                adminReport.MenteeThreeYearCount = new List<int>();
                for (int i = 2; i >= 0; i--)
                {
                    adminReport.MenteeThreeYearCount
                        .Add(users.Where(a => a.Role == UserRole.MENTEE && a.CreatedDate.Year == DateTime.Now.Year - i).Count());
                }

                //Request
                IEnumerable<Request> requests = (await work.Requests.GetAllAsync()).Where(a => !a.IsDeleted);
                adminReport.RequestThreeYearCount = new List<int>();
                for (int i = 2; i >= 0; i--)
                {
                    adminReport.RequestThreeYearCount
                        .Add(requests.Where(a => a.CreatedDate.Year == DateTime.Now.Year - i).Count());
                }

                //Seminar
                IEnumerable<Seminar> seminars = (await work.Seminars.GetAllAsync()).Where(a => !a.IsDeleted);
                adminReport.SeminarThreeYearCount = new List<int>();
                for (int i = 2; i >= 0; i--)
                {
                    adminReport.SeminarThreeYearCount
                        .Add(seminars.Where(a => a.CreatedDate.Year == DateTime.Now.Year - i).Count());
                }
            }
            #endregion

            #region Right
            //Top skills
            string sql = "select Skills.* from Skills inner join ( " +
                         "select top(3) s.Id, count(s.Id) as countSkill from Requests r " +
                         "left join RequestSkills rs " +
                         "on r.Id = rs.RequestId " +
                         "left join Skills s " +
                         "on rs.SkillId = s.Id " +
                         "group by s.Id " +
                         "order by countSkill desc) a " +
                         "on a.Id = Skills.Id;";
            List<Skill> top3Skills = (await work.Skills.ExecuteQueryAsync(sql)).ToList();
            adminReport.TopSkills = new List<string>();
            foreach (var a in top3Skills)
            {
                adminReport.TopSkills.Add(a.Name);
            }
            //Top mentors
            sql = "select u.* " +
                        "from Requests r inner join Users u " +
                        "on r.MentorId = u.Id " +
                        "where r.IsDeleted = 0 " +
                        "and year(r.CreatedDate) = year(getdate()) " +
                        "and month(r.CreatedDate) = month(getdate()); ";
            List<User> listMentors = (await work.Users.ExecuteQueryAsync(sql)).ToList();
            var top3Mentors = listMentors.GroupBy(x => x.Name)
                .Select(m => new { Name = m.Key, Count = m.Count() })
                .OrderByDescending(m => m.Count).Take(3).ToList();
            adminReport.TopMentors = new List<string>();
            foreach (var a in top3Mentors)
            {
                adminReport.TopMentors.Add(a.Name);
            }

            //Top ratings
            sql = "select Users.* from ( " +
                       "select top(3) u.Id, u.Name, u.CreatedDate, avg(cast(r.NoOfStar as float)) as avgStar " +
                       "from Users u inner join Ratings r " +
                       "on u.Id = r.MentorId " +
                       "group by u.Name, u.Id, u.CreatedDate " +
                       "order by avgStar desc ) a " +
                    "left join Users " +
                    "on a.Id = Users.Id";
            List<User> top3MentorRateds = (await work.Users.ExecuteQueryAsync(sql)).ToList();
            adminReport.TopRatings = new List<string>();
            foreach (var a in top3MentorRateds)
            {
                adminReport.TopRatings.Add(a.Name);
            }
            #endregion

            return adminReport;
        }
    }

}
