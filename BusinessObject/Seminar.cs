using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjectLibrary
{
    public partial class Seminar
    {
        public Seminar()
        {
            Ratings = new HashSet<Rating>();
            SeminarParticipants = new HashSet<SeminarParticipant>();
        }

        public string Id { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public string MentorId { get; set; }
        public string MeetingUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public bool IsDeleted { get; set; }

        public virtual MentorDetail Mentor { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<SeminarParticipant> SeminarParticipants { get; set; }
    }
}
