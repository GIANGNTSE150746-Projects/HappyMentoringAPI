using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsLibrary
{
    public static class HmsUtils
    {
        public static string CreateGuid()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }

        public static List<DateTime> GetDaysOfCurrentMonth()
        { 
            DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var dates = Enumerable.Range(1, DateTime.DaysInMonth(date.Year, date.Month))
                .Select(n => new DateTime(date.Year, date.Month, n));
            var weeks = from d in dates
                           select d;
            return weeks.ToList();
        }
    }
}
