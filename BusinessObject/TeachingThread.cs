using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjectLibrary
{
    public partial class TeachingThread
    {
        public string Id { get; set; }
        public string RequestId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Request Request { get; set; }
    }
}
