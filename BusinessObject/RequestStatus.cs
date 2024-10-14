using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLibrary
{
    public enum RequestStatus
    {
        PENDING = 0,
        ACCEPTED = 1,
        REJECTED = 2,
        CONCLUDED = 3,
        REPORTED = 4,
        INVALID = 5
    }
}
