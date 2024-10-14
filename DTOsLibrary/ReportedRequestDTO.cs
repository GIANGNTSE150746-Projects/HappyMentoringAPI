using BusinessObjectLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLibrary
{
    public class ReportedRequestDTO
    {
        public string RequestId { get; set; }
        public string Topic { get; set; }
        public string Content { get; set; }
        public string MenteeName { get; set; }
        public string MentorName { get; set; }
        public RequestStatus Status { get; set; }
    }
}
