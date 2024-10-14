using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObjectLibrary
{
    public partial class SeminarParticipant
    {
        [Key]
        public string SeminarId { get; set; }
        [Key]
        public string UserId { get; set; }

        public virtual Seminar Seminar { get; set; }
        public virtual User User { get; set; }
    }
}
