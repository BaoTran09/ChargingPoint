using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ChargingApp.Helpers
{
    public static class DatabaseHelper
    {
        private static string _connectionString;

        static DatabaseHelper()
        {
            try
            {
                _connectionString = ConfigurationManager.ConnectionStrings["ChargingAppDb"].ConnectionString;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể đọc chuỗi kết nối từ file cấu hình.", ex);
            }
        }

        // Lấy kết nối mới
        public static SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);

            // Mở kết nối nếu chưa mở
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            return connection;
        }

        // Thực thi câu lệnh không trả về dữ liệu (INSERT, UPDATE, DELETE)
        public static int ExecuteNonQuery(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.CommandType = commandType;

                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                try
                {
                    return command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi khi thực thi câu lệnh SQL", ex);
                }
            }
        }

        // Thực thi câu lệnh trả về một giá trị đơn
        public static object ExecuteScalar(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.CommandType = commandType;

                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                try
                {
                    return command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi khi thực thi câu lệnh SQL", ex);
                }
            }
        }

        // Thực thi câu lệnh trả về DataTable
        public static DataTable ExecuteDataTable(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.CommandType = commandType;

                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                try
                {
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi khi thực thi câu lệnh SQL", ex);
                }
            }
        }

        // Thực thi câu lệnh trả về DataSet
        public static DataSet ExecuteDataSet(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.CommandType = commandType;

                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                try
                {
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataSet = new DataSet();
                        adapter.Fill(dataSet);
                        return dataSet;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi khi thực thi câu lệnh SQL", ex);
                }
            }
        }

        // Thực thi stored procedure
        public static DataTable ExecuteStoredProcedure(string procedureName, params SqlParameter[] parameters)
        {
            return ExecuteDataTable(procedureName, CommandType.StoredProcedure, parameters);
        }
    }
}