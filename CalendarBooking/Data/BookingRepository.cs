using CalendarBooking.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CalendarBooking.Data
{
    public class BookingRepository : IBooking
    {
        private readonly BookingDBContext _context;

        public BookingRepository(BookingDBContext context)
        {
            _context = context;
        }

        public async Task<BookingModel> Add(BookingModel model)
        {
            var sql = $@"INSERT INTO [dbo].[Booking]
                                ([PeriodStart],
                                 [PeriodEnd],
                                 [Status])
                                VALUES
                                (@PeriodStart,
                                 @PeriodEnd,
                                 @Status); SELECT CAST(SCOPE_IDENTITY() as int)";

            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<int>(sql,
                                          new { model.PeriodStart, model.PeriodEnd, model.Status });
            model.Id = result.Single();
            return model;
        }

        public async Task<int> Get(BookingModel model)
        {
            var sql = $@"SELECT CAST([Id] as int)
                            FROM
                               [Booking] WHERE [PeriodStart]=@PeriodStart AND [PeriodEnd]=@PeriodEnd AND ([Status]=@Status OR [Status]=@ReservedStatus)";
            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<int>(sql, new { model.PeriodStart, model.PeriodEnd, model.Status, @ReservedStatus = "Reserved" });
            if (result.Count() > 0)
                return result.Single();
            else
                return -1;
        }

        public async Task<IEnumerable<BookingModel>> Find(DateTime start, DateTime end)
        {
            var sql = $@"SELECT [Id],
                               [PeriodStart],
                               [PeriodEnd],
                               [Status]
                            FROM
                               [Booking] WHERE [PeriodStart]>=@PeriodStart AND [PeriodEnd]<=@PeriodEnd";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<BookingModel>(sql, new { @PeriodStart = start, @PeriodEnd = end });
        }

        public async Task<BookingModel> Remove(BookingModel model)
        {
            var sql = $@"
                        DELETE FROM
                            [dbo].[Booking]
                        WHERE
                            [Id]=@Id";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, model);
            return model;
        }
    }
}