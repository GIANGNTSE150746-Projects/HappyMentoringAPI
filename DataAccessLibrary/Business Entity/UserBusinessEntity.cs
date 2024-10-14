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
    public class UserBusinessEntity
    {
        private IUnitOfWork work;
        public UserBusinessEntity(IUnitOfWork work)
        {
            this.work = work;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            IEnumerable<User> users = (await work.Users.GetAllAsync(nameof(User.MentorDetail)))
                .Where(u => !u.IsDeleted);

            foreach (User user in users)
            {
                if (user.Role == UserRole.MENTOR)
                {
                    if (user.MentorDetail != null)
                    {
                        user.MentorDetail.MentorSkills =
                            (await work.MentorSkills.GetAllAsync(nameof(MentorSkill.Skill)))
                            .Where(mentorSkill => mentorSkill.MentorId.Equals(user.Id))
                            .ToList();
                        user.Ratings = (await work.Ratings.GetAllAsync())
                            .Where(r => r.MentorId.Equals(user.Id))
                            .ToList();
                    }
                }
            }

            return users;

        }

        public async Task<IEnumerable<MentorDTO>> GetMentorsAsync()
        {
            IEnumerable<User> mentorsList = (await work.Users.GetAllAsync(nameof(User.MentorDetail)))
                .Where(u => !u.IsDeleted && u.Role == UserRole.MENTOR);
            IEnumerable<Cv> cvs = (await work.Cvs.GetAllAsync()).Where(c => !c.IsDeleted);
            List<MentorDTO> mentors = new List<MentorDTO>();
            foreach (User mentor in mentorsList)
            {
                //Get skills of mentor
                string query = "select s.* from MentorSkills ms " +
                               "inner join Skills s " +
                               "on ms.SkillId = s.Id " +
                               "inner join  MentorDetails md " +
                               "on md.MentorId = ms.MentorId " +
                               "where ms.MentorId = '" + mentor.Id + "'";
                IList<Skill> skills = (await work.Skills.ExecuteQueryAsync(query)).ToList();

                IEnumerable<MentorSkill> mentorSkills = new List<MentorSkill>();

                string mentorSkillsStr = "";

                for (int i = 0; i < skills.Count(); i++)
                {
                    if (i == skills.Count() - 1)
                    {
                        mentorSkillsStr += skills[i].Name;
                    }
                    else
                    {
                        mentorSkillsStr += skills[i].Name + ", ";
                    }
                    mentorSkills = mentorSkills.Append(new MentorSkill()
                    {
                        Skill = skills[i]
                    });
                }

                //mentor.MentorDetail.MentorSkills = mentorSkills.ToList();

                //Get rating of mentor
                int ratingCount = 0;
                double avgRating = 0;
                IEnumerable<Rating> ratings = (await work.Ratings.GetAllAsync())
                    .Where(r => r.MentorId == mentor.Id);
                if (ratings.Count() > 0)
                {
                    foreach (var rating in ratings)
                    {
                        ratingCount += rating.NoOfStar;
                    }
                    avgRating = (double)ratingCount / ratings.Count();
                }
                else
                {
                    avgRating = 0;
                }

                //Get working status of mentor
                MentorDetail mentorDetail = await work.MentorDetails.GetAsync(mentor.Id);

                //Get Cv URL
                string CvUrl = cvs.Where(c => c.MentorId == mentor.Id).FirstOrDefault() != null ?
                    cvs.Where(c => c.MentorId == mentor.Id).FirstOrDefault().Description : "";

                //Get Salary
                double baseSalary = 4680000;

                IEnumerable<Request> requests = (await work.Requests.GetAllAsync())
                    .Where(r => !r.IsDeleted && r.MentorId == mentor.Id &&
                    r.CreatedDate.Month == DateTime.Now.Month);
                int resovledRequests = requests.Where(r => r.Status == RequestStatus.CONCLUDED).Count();
                int acceptedRequests = requests.Where(r => r.Status == RequestStatus.ACCEPTED).Count();
                double bonus = baseSalary * ((double)resovledRequests / (acceptedRequests != 0 ? acceptedRequests : 1 + resovledRequests));

                double salary = baseSalary + bonus * (avgRating * 0.6);

                mentors.Add(new MentorDTO
                {
                    Id = mentor.Id,
                    Email = mentor.Email,
                    Name = mentor.Name,
                    Phone = mentor.Phone,
                    Skills = mentorSkillsStr,
                    Rating = avgRating,
                    WorkingStatus = mentorDetail.WorkStatus,
                    CvUrl = CvUrl,
                    MentorSkills = mentorSkills.ToList(),
                    Salary = salary
                });
            }
            return mentors.OrderBy(m => m.WorkingStatus);
        }

        public async Task<IEnumerable<User>> GetMenteesAsync()
        {
            return (await work.Users.GetAllAsync())
                .Where(u => !u.IsDeleted && u.Role == UserRole.MENTEE);
        }
        public async Task<User> GetUserAsync(string id)
        {
            return (await work.Users.GetAllAsync())
                .Where(u => !u.IsDeleted)
                .Where(u => u.Id.Equals(id.Trim().ToLower()))
                .FirstOrDefault();
        }

        public async Task<UserDTO> GetUserByEmailAsync(string email)
        {
            var res = new UserDTO
            {
                user = (await work.Users.GetAllAsync())
                .Where(u => !u.IsDeleted)
                .Where(u => u.Email.Equals(email.Trim().ToLower()))
                .FirstOrDefault()
            };
            if (res.user != null && res.user.Role == UserRole.MENTOR)
            {
                var query = (await work.Users.GetAllAsync())
                    .Join(await work.MentorSkills.GetAllAsync(),
                    user => user.Id,
                    ms => ms.MentorId,
                    (user, ms) => new
                    {
                        user = user,
                        mentorSkill = ms
                    }
                    ).ToList();
                var r = query.Where(u => !u.user.IsDeleted && u.user.Email.Equals(email.Trim().ToLower()))
                    .FirstOrDefault();
                if (r.user != null)
                {
                    List<MentorSkill> mentorSkills = new List<MentorSkill>();
                    foreach (var q in query)
                    {
                    
                            if (q.user.Id == r.user.Id)
                            {
                                var skill = await work.Skills.GetAsync(q.mentorSkill.SkillId);
                                q.mentorSkill.Skill = skill;
                                mentorSkills.Add(q.mentorSkill);
                            }
                    }
                    MentorDetail md = new MentorDetail();
                    if (r.user.Role == UserRole.MENTOR)
                    {
                        md = await work.MentorDetails.GetAsync(r.user.Id);
                        md.Cvs = (await work.Cvs.GetAllAsync()).Where(cv => cv.MentorId == r.user.Id).ToList();
                        r.user.Ratings = (await work.Ratings.GetAllAsync())
                            .Where(rating => rating.MentorId.Equals(r.user.Id))
                            .ToList();
                        r.user.MentorDetail = md;
                    }
                    res = new UserDTO
                    {
                        user = r.user,
                        mentorSkills = mentorSkills,
                    };
                }
            }
            return res;

        }

        //public async Task<User> MenteeUploadCv(string menteeId, string cvUrl)
        //{
        //    User user = await work.Users.GetAsync(menteeId);
        //    if (user == null)
        //    {
        //        throw new ApplicationException("User does not exist!!");
        //    } else
        //    {
        //        if (user.Role != UserRole.MENTEE)
        //        {
        //            throw new ApplicationException("User Role is not valid to do this operation!!");
        //        }
        //        Cv cv = new Cv()
        //        {
        //            Id = HmsUtils.CreateGuid(),
        //            Description = cvUrl,
        //            CreatedDate = DateTime.Now,
        //            IsDeleted = false,
        //            MentorId = user.Id
        //        };

        //        MentorDetail mentorDetail = new MentorDetail()
        //        {
        //            MentorId = user.Id,
        //            Cvs = new List<Cv>() { cv },
        //            WorkStatus = MentorWorkingStatus.PENDING
        //        };

        //        user.MentorDetail = mentorDetail;
        //        user.Role = UserRole.MENTOR;

        //        work.Users.Update(user);
        //        await work.Save();
        //        return user;
        //    }
        //}

        public async Task<User> AddUser(User user)
        {
            User existedUser = (await work.Users.GetAllAsync(true))
                .SingleOrDefault(u => u.Email.ToLower().Equals(user.Email.ToLower()));
            if (existedUser != null)
            {
                throw new ApplicationException($"User with email {user.Email} is existed!! Please try with another email address.");
            }

            string id = HmsUtils.CreateGuid();
            user.Id = id;
            user.IsDeleted = false;
            user.CreatedDate = DateTime.Now;
            if (user.Role == UserRole.MENTOR)
            {
                Cv cv = user.MentorDetail.Cvs.FirstOrDefault();
                cv.Id = HmsUtils.CreateGuid();
                cv.MentorId = user.Id;
                //cv.Description = user.MentorDetail.Cvs.FirstOrDefault().Description;
                cv.IsDeleted = false;
                cv.CreatedDate = user.CreatedDate;
                user.MentorDetail.WorkStatus = MentorWorkingStatus.PENDING;
            }
            else if (user.MentorDetail != null && user.MentorDetail.Cvs.Any())
            {
                throw new ApplicationException("Mentee is not allowed to have CV!!");
            }
            //user.MentorDetail.Cvs = null;
            await work.Users.AddAsync(user);
            //await work.Cvs.AddAsync(cv);
            await work.Save();
            return user;
        }

        public async Task<User> UpdateUser(User user)
        {
            //IEnumerable<User> users = await work.Users.GetAllAsync();

            //users = users.Where(user => user.Id.Equals(user.Id));
            User existedUser = (await work.Users.GetAllAsync(false))
                .SingleOrDefault(u => !u.IsDeleted && u.Id.Equals(user.Id));

            if (existedUser == null)
            {
                throw new ApplicationException("User does not exist!!");
            }
            if (!existedUser.Email.ToLower().Equals(user.Email.ToLower()))
            {
                throw new ApplicationException("User email is not allowed to be changed!! Please try again...");
            }

            if (user.Phone != null)
            {
                user.Phone.IsPhoneNumber("Invalid phone number");
            }

            if (user.MentorDetail != null)
            {
                List<MentorSkill> mentorSkills = (await work.MentorSkills.GetAllAsync())
                .Where(ms => ms.MentorId == user.Id).ToList();
                if (user.MentorDetail.MentorSkills != null)
                {
                    foreach (var ms in mentorSkills)
                    {
                        work.MentorSkills.Delete(ms);
                    }
                    foreach (var newMs in user.MentorDetail.MentorSkills)
                    {
                        newMs.Id = HmsUtils.CreateGuid();
                        newMs.Skill = null;
                        await work.MentorSkills.AddAsync(newMs);
                    }
                }
            }
            work.Users.Update(user);
            await work.Save();
            return user;
        }

        public async Task<User> UpdateMentor(MentorDTO mentor)
        {
            await isValidMentor(mentor);
            //Update User -> email, name, phone
            User user = await work.Users.GetAsync(mentor.Id);
            user.Email = mentor.Email;
            user.Name = mentor.Name;
            user.Phone = mentor.Phone;

            work.Users.Update(user);

            //Update MentorSkills: delete existed list -> add new list
            List<MentorSkill> mentorSkills = (await work.MentorSkills.GetAllAsync())
                .Where(ms => ms.MentorId == mentor.Id).ToList();
            if (mentor.MentorSkills != null)
            {
                foreach (var ms in mentorSkills)
                {
                    work.MentorSkills.Delete(ms);
                }
                foreach (var newMs in mentor.MentorSkills)
                {
                    newMs.Id = HmsUtils.CreateGuid();
                    newMs.Skill = null;
                    await work.MentorSkills.AddAsync(newMs);
                }
            }

            //Update MentorDetails -> Working Status
            MentorDetail currentDetail = await work.MentorDetails.GetAsync(mentor.Id);
            if (currentDetail != null)
            {
                currentDetail.WorkStatus = mentor.WorkingStatus;
                work.MentorDetails.Update(currentDetail);
            }

            await work.Save();
            return user;
        }

        private async Task isValidMentor(MentorDTO mentor)
        {
            User existedUser = (await work.Users.GetAllAsync())
                .SingleOrDefault(user => !user.IsDeleted && user.Id.Equals(user.Id));

            if (existedUser == null)
            {
                throw new ApplicationException("User does not exist!!");
            }
            if (!existedUser.Email.ToLower().Equals(mentor.Email.ToLower()))
            {
                throw new ApplicationException("User email is not allowed to be changed!! Please try again...");
            }

            mentor.Email.IsEmail("Invalid email");

            if (mentor.Phone != null)
            {
                mentor.Phone.IsPhoneNumber("Invalid phone number");
            }
        }


        public async Task RemoveUser(string id)
        {
            User user = await work.Users.GetAsync(id);
            user.IsDeleted = true;
            work.Users.Update(user);
            await work.Save();
        }

        public async Task<User> Login(string email, string password)
        {
            User user = null;
            user = (await work.Users.GetAllAsync())
                    .SingleOrDefault(u => !u.IsDeleted && u.Email.Equals(email.ToLower())
                                && u.Password.Equals(password));

            if (user != null)
            {
                if (user.Role == UserRole.MENTOR)
                {
                    MentorDetail mentorDetail = (await work.MentorDetails.GetAllAsync())
                        .SingleOrDefault(md => md.MentorId.Equals(user.Id));

                    user.MentorDetail = mentorDetail;
                }
            }
            
            return user;
        }

        //private async Task CheckUser(User user)
        //{
            
        //}
    }
}
