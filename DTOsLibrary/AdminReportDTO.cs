using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLibrary
{
    public class AdminReportDTO
    {
        public string Filter { get; set; }

        //Top
        public int MentorCount { get; set; }
        public float MentorPercentage { get; set; }
        public int MenteeCount { get; set; }
        public float MenteePercentage { get; set; }
        public int RequestCount { get; set; }
        public float RequestPercentage { get; set; }
        public int SeminarCount { get; set; }
        public float SeminarPercentage { get; set; }

        //Main
        //public List<int> MentorCountSixMonthsRange { get; set; }
        //public List<int> MenteeCountSixMonthsRange { get; set; }
        //public List<int> RequestCountSixMonthsRange { get; set; }
        //public List<int> SeminarCountSixMonthsRange { get; set; }
        public List<int> MentorWeekCount { get; set; }
        public List<int> MenteeWeekCount { get; set; }
        public List<int> RequestWeekCount { get; set; }
        public List<int> SeminarWeekCount { get; set; }

        public List<int> MentorMonthCount { get; set; }
        public List<int> MenteeMonthCount { get; set; }
        public List<int> RequestMonthCount { get; set; }
        public List<int> SeminarMonthCount { get; set; }

        public List<int> MentorThreeYearCount { get; set; }
        public List<int> MenteeThreeYearCount { get; set; }
        public List<int> RequestThreeYearCount { get; set; }
        public List<int> SeminarThreeYearCount { get; set; }


        //Right
        public List<string> TopSkills { get; set; }
        public List<string> TopMentors { get; set; }
        public List<string> TopRatings { get; set; }
    }
}
