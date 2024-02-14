using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Order.Outbox.Table.Publisher.Service
{
    public static class OrderOutboxSingletonDatabase
	{
		static IDbConnection _connection;
        static bool _dataReaderState = true;

        static OrderOutboxSingletonDatabase() => _connection = new SqlConnection("Server=localhost, 1433;Database=OrderAPIDB-OUTBOX;User ID=SA;Password=Sefa-1234;TrustServerCertificate=True;");

        public static IDbConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        public static async Task<IEnumerable<T>> QueryAsync<T>(string sql)
		{
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
			return await _connection.QueryAsync<T>(sql);
		}

        public static async Task<int> ExecuteAsync(string sql)
        {
            return await _connection.ExecuteAsync(sql);
        }

        public static void DataReaderReady()
        {
            _dataReaderState = true;
        }

        public static void DataReaderReadyBusy()
        {
            _dataReaderState = false;
        }

        public static bool DataReaderState => _dataReaderState;
    }   
}

