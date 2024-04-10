using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarBooking.Data
{
    public class BookingDBContext
    {
        private readonly IConfiguration _config;
        private readonly string? _connection;

        public BookingDBContext(IConfiguration config)
        {
            _config = config;
            _connection = _config.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connection);
    }
}