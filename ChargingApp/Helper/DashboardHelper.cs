using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ChargingApp.Helper
{
    public static class DashboardHelper
    {
        private static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=ChargingPoint;Integrated Security=True;Connect Timeout=30";

        #region Dashboard Summary

        public static DataTable GetDashboardSummary(DateTime? fromDate = null, DateTime? toDate = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetDashboardSummary", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (fromDate.HasValue)
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Value);

                if (toDate.HasValue)
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Value);

                DataTable dt = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                return dt;
            }
        }

        #endregion

        #region Revenue Charts

        public static DataTable GetRevenueByDate(DateTime? fromDate = null, DateTime? toDate = null, string groupBy = "Day")
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetRevenueByDate", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (fromDate.HasValue)
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Value);

                if (toDate.HasValue)
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Value);

                cmd.Parameters.AddWithValue("@GroupBy", groupBy);

                DataTable dt = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                return dt;
            }
        }

        public static DataTable GetRevenueByStation(DateTime? fromDate = null, DateTime? toDate = null, int top = 10)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetRevenueByStation", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (fromDate.HasValue)
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Value);

                if (toDate.HasValue)
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Value);

                cmd.Parameters.AddWithValue("@Top", top);

                DataTable dt = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                return dt;
            }
        }

        #endregion

        #region Charger & Status Stats

        public static DataTable GetChargerUsageStats(DateTime? fromDate = null, DateTime? toDate = null, int top = 10)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetChargerUsageStats", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (fromDate.HasValue)
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Value);

                if (toDate.HasValue)
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Value);

                cmd.Parameters.AddWithValue("@Top", top);

                DataTable dt = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                return dt;
            }
        }

        public static DataTable GetInvoiceStatusStats(DateTime? fromDate = null, DateTime? toDate = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetInvoiceStatusStats", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (fromDate.HasValue)
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Value);

                if (toDate.HasValue)
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Value);

                DataTable dt = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                return dt;
            }
        }

        #endregion

        #region Bonus Stats

        public static DataTable GetRevenueByVehicleModel(DateTime? fromDate = null, DateTime? toDate = null, int top = 10)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetRevenueByVehicleModel", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (fromDate.HasValue)
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Value);

                if (toDate.HasValue)
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Value);

                cmd.Parameters.AddWithValue("@Top", top);

                DataTable dt = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                return dt;
            }
        }

        public static DataTable GetTopCustomers(DateTime? fromDate = null, DateTime? toDate = null, int top = 10)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetTopCustomers", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (fromDate.HasValue)
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Value);

                if (toDate.HasValue)
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Value);

                cmd.Parameters.AddWithValue("@Top", top);

                DataTable dt = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                return dt;
            }
        }

        #endregion

        #region Active Sessions

        public static DataTable GetActiveSessionsStats()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetActiveSessionsStats", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable dt = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                return dt;
            }
        }

        public static DataTable GetActiveChargingSessions()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetActiveChargingSessions", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable dt = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                return dt;
            }
        }

        #endregion
    }
}