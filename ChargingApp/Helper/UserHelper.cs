using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChargingApp.Helper;
using ChargingApp.Helpers;


namespace ChargingApp.Helper
{
    public static class UserHelper
    {
        private static readonly string connectionString =
            @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=ChargingPoint;Integrated Security=True";

        // Lấy UserAppId sau khi user login
        public static long GetUserAppId(string username)
        {
            string query = @"SELECT UserId_App FROM User_App WHERE Username = @Username";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                conn.Open();

                object result = cmd.ExecuteScalar();

                if (result == null)
                    throw new Exception("Không tìm thấy UserAppId cho username: " + username);

                return Convert.ToInt64(result);
            }
        }

        // Lấy EmployeeId từ UserAppId
        public static long? GetEmployeeIdFromUser(long userAppId)
        {
            string query = @"SELECT EmployeeId FROM Employee WHERE UserAppId = @UserAppId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserAppId", userAppId);
                conn.Open();

                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    return null;  // Tài khoản không gắn Employee

                return Convert.ToInt64(result);
            }
        
        }
    }









}
