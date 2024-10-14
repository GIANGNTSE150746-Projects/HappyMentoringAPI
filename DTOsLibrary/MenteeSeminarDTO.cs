using BusinessObjectLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLibrary
{
    public class MenteeSeminarDTO
    {
        public IEnumerable<Seminar> MenteeSeminars { get; set; }
        public IEnumerable<Seminar> UpcomingSeminars { get; set; }

    }

    public class MentorSeminarDTO
    {
        public IEnumerable<Seminar> MentorSeminars { get; set; }
        public IEnumerable<Seminar> UpcomingSeminars { get; set; }
    }

    public class MentorOtherSeminarDTO
    {
        public IEnumerable<Seminar> MentorSeminars { get; set; }
        public IEnumerable<Seminar> UpcomingSeminars { get; set; }
    }
}
