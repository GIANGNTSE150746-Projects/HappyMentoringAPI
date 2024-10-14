using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjectLibrary
{
    public partial class Cv
    {
        public string Id { get; set; }
        public string MentorId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual MentorDetail Mentor { get; set; }
    }
}
