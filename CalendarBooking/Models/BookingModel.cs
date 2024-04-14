using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarBooking.Models
{
    public class BookingModel
    {
        private const string NAME = "Venkat Penuganti";
        private static readonly DateTime TODAY = DateTime.Now;

        public long Id { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string? Status { get; set; }
        public string? CreatedBy { get; set; } = NAME;
        public DateTime CreatedDate { get; set; } = TODAY;
        public string? ModifiedBy { get; set; } = NAME;
        public DateTime ModifiedDate { get; set; } = TODAY;
    }
}