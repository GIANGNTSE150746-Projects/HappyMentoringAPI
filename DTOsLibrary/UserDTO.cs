using BusinessObjectLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLibrary
{
    public class UserDTO
    {
        public User user { get; set; }
        public IEnumerable<MentorSkill> mentorSkills { get; set; }
    }
}
