using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObjectLibrary
{
    public partial class MentorDetail
    {
        public MentorDetail()
        {
            Cvs = new HashSet<Cv>();
            MentorSkills = new HashSet<MentorSkill>();
            Ratings = new HashSet<Rating>();
            Requests = new HashSet<Request>();
            Seminars = new HashSet<Seminar>();
        }


        [Key]
        public string MentorId { get; set; }
        public decimal? Salary { get; set; }
        public MentorWorkingStatus WorkStatus { get; set; }

        public virtual User Mentor { get; set; }
        public virtual ICollection<Cv> Cvs { get; set; }
        public virtual ICollection<MentorSkill> MentorSkills { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
        public virtual ICollection<Seminar> Seminars { get; set; }
    }
}
