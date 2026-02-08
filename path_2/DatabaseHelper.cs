using MySql.Data.MySqlClient;
using System;
using System.Configuration;

namespace path_2
{
    /// <summary>
    /// Helper class để quản lý kết nối database
    /// </summary>
    public static class DatabaseHelper
    {
        // Connection string từ App.config
        private static string connectionString = "server=localhost;uid=root;pwd=thuymv;database=path_1;charset=utf8mb4;";

        /// <summary>
        /// Lấy connection mới
        /// </summary>
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Test kết nối database
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
