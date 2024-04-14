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
    /// <summary>
    /// BookingDBContext
    ///  - Manages data access
    /// </summary>
    public class BookingDBContext
    {
        private readonly IConfiguration _config;
        private readonly string? _connection;
        private const string CONNECTION_STRING = "DefaultConnection";

        /// <summary>
        /// Constructor - Initializes BookingDBContext
        /// </summary>
        /// <param name="config"></param>
        public BookingDBContext(IConfiguration config)
        {
            _config = config;
            _connection = _config.GetConnectionString(CONNECTION_STRING);
        }

        /// <summary>
        /// Creates Connection
        /// </summary>
        /// <returns>IDbConnections</returns>
        public IDbConnection CreateConnection() => new SqlConnection(_connection);
    }
}