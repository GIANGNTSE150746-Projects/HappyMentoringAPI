using BusinessObjectLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLibrary
{
    public class MentorDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Skills { get; set; }
        public double Rating { get; set; }
        public MentorWorkingStatus WorkingStatus { get; set; }
        public string CvUrl { get; set; }
        public double Salary { get; set; }

        public virtual ICollection<MentorSkill> MentorSkills { get; set; }
    }

    public class Mentor_RegisterDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Skills { get; set; }
        public double Rating { get; set; }
        public string CvUrl { get; set; }
        public MentorWorkingStatus WorkingStatus { get; set; }

        public virtual ICollection<MentorSkill> MentorSkills { get; set; }
    }
}
