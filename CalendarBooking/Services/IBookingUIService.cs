using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarBooking.Services
{
    public interface IBookingUIService
    {
        private const string EMPTY = "";

        void Run(string optionalInput = EMPTY);
    }
}