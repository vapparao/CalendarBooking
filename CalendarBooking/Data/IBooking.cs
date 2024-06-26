﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalendarBooking.Models;

namespace CalendarBooking.Data
{
    public interface IBooking
    {
        Task<int> Get(BookingModel model);

        Task<IEnumerable<BookingModel>> Find(DateTime start, DateTime end);

        Task<BookingModel> Add(BookingModel model);

        Task<BookingModel> Remove(BookingModel model);
    }
}