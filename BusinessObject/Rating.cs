using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjectLibrary
{
    public partial class Rating
    {
        public string Id { get; set; }
        public int NoOfStar { get; set; }
        public string Comments { get; set; }
        public string MentorId { get; set; }
        public string MenteeId { get; set; }
        public string RequestId { get; set; }
        public string SeminarId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual User Mentee { get; set; }
        public virtual MentorDetail Mentor { get; set; }
        public virtual Request Request { get; set; }
        public virtual Seminar Seminar { get; set; }
    }
}
