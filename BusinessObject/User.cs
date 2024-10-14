using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObjectLibrary
{
    public partial class User
    {
        public User()
        {
            Ratings = new HashSet<Rating>();
            Requests = new HashSet<Request>();
            SeminarParticipants = new HashSet<SeminarParticipant>();
        }

        public string Id { get; set; }

        [Required(ErrorMessage = "User Email is required!!")]
        [EmailAddress(ErrorMessage = "User Email is not a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "User Password is required!!")]
        [MinLength(8, ErrorMessage = "User Password requires at least 8 characters!!")]
        [MaxLength(20, ErrorMessage = "User Password is limited to 20 characters!!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "User Name is required!!")]
        [MaxLength(50, ErrorMessage = "User Name is limited to 50 characters!!")]
        public string Name { get; set; }

        [Phone(ErrorMessage = "User Phone number is not a valid phone number!!")]
        public string Phone { get; set; }

        public UserRole Role { get; set; }
        public string Image { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual MentorDetail MentorDetail { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
        public virtual ICollection<SeminarParticipant> SeminarParticipants { get; set; }
    }
}
