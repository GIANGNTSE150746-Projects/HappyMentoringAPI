using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjectLibrary
{
    public partial class Skill
    {
        public Skill()
        {
            MentorSkills = new HashSet<MentorSkill>();
            RequestSkills = new HashSet<RequestSkill>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<MentorSkill> MentorSkills { get; set; }
        public virtual ICollection<RequestSkill> RequestSkills { get; set; }
    }
}
