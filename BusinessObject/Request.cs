using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjectLibrary
{
    public partial class Request
    {
        public Request()
        {
            Ratings = new HashSet<Rating>();
            RequestSkills = new HashSet<RequestSkill>();
            TeachingThreads = new HashSet<TeachingThread>();
        }

        public string Id { get; set; }
        public string Topic { get; set; }
        public string Content { get; set; }
        public string MenteeId { get; set; }
        public string MentorId { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? CanceledDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime? ReportedDate { get; set; }

        public bool IsDeleted { get; set; }

        public virtual User Mentee { get; set; }
        public MentorDetail Mentor { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<RequestSkill> RequestSkills { get; set; }
        public virtual ICollection<TeachingThread> TeachingThreads { get; set; }
    }
}
