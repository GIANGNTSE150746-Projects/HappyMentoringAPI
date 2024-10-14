using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjectLibrary
{
    public partial class MentorSkill
    {
        public string Id { get; set; }
        public string MentorId { get; set; }
        public string SkillId { get; set; }
        public int YearsExperience { get; set; }
        public int Rate { get; set; }

        public virtual MentorDetail Mentor { get; set; }
        public virtual Skill Skill { get; set; }
    }
}
