using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObjectLibrary
{
    public partial class RequestSkill
    {
        [Key]
        public string RequestId { get; set; }
        [Key]
        public string SkillId { get; set; }

        public virtual Request Request { get; set; }
        public virtual Skill Skill { get; set; }
    }
}
