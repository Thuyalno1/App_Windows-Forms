using System;
using System.Data;
using System.Data.Odbc;
using System.Windows.Forms;

namespace path_2
{
    /// <summary>
    /// Helper class để quản lý kết nối database qua ODBC
    /// </summary>
    public static class DatabaseHelper
    {
        // Connection string sử dụng DSN
        private static string connectionString = "DSN=MySQL_PATH1;DATABASE=path_1;";
        
        // Public variables giống như VB.NET module
        public static OdbcConnection connection;
        public static OdbcCommand dml;
        public static OdbcDataReader dr;
        public static string sql;

        /// <summary>
        /// Kết nối đến database qua ODBC DSN
        /// </summary>
        public static void ConnectDB()
        {
            try
            {
                connection = new OdbcConnection(connectionString);
                // Mở kết nối nếu đang đóng
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
            }
            catch (OdbcException odbcEx)
            {
                // Bắt lỗi ODBC cụ thể
                string errorMessage = $"[{Constants.ERR_DB_CONNECTION}] ODBC Error:\n";
                foreach (OdbcError error in odbcEx.Errors)
                {
                    errorMessage += $"Message: {error.Message}\n";
                    errorMessage += $"Native Error: {error.NativeError}\n";
                    errorMessage += $"Source: {error.Source}\n";
                    errorMessage += $"SQL State: {error.SQLState}\n\n";
                }
                MessageBox.Show(errorMessage, Constants.DbMessages.CONNECTION_FAILED, 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Constants.DbMessages.CONNECTION_FAILED}\n{ex.Message}", 
                    $"Lỗi {Constants.ERR_DB_CONNECTION}", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        /// <summary>
        /// Lấy connection mới
        /// </summary>
        public static OdbcConnection GetConnection()
        {
            try
            {
                return new OdbcConnection(connectionString);
            }
            catch (OdbcException odbcEx)
            {
                HandleOdbcException(odbcEx, "Lấy kết nối");
                throw;
            }
        }

        /// <summary>
        /// Test kết nối database
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using (OdbcConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (OdbcException odbcEx)
            {
                HandleOdbcException(odbcEx, "Kiểm tra kết nối");
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Đóng kết nối database
        /// </summary>
        public static void CloseConnection()
        {
            try
            {
                if (dr != null && !dr.IsClosed)
                {
                    dr.Close();
                }
                
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            catch (OdbcException odbcEx)
            {
                HandleOdbcException(odbcEx, "Đóng kết nối");
            }
        }

        /// <summary>
        /// Thực thi câu lệnh SQL (INSERT, UPDATE, DELETE)
        /// </summary>
        public static int ExecuteNonQuery(string sqlCommand)
        {
            try
            {
                using (OdbcConnection conn = GetConnection())
                {
                    conn.Open();
                    using (OdbcCommand cmd = new OdbcCommand(sqlCommand, conn))
                    {
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (OdbcException odbcEx)
            {
                HandleOdbcException(odbcEx, "Thực thi câu lệnh SQL");
                throw;
            }
        }

        /// <summary>
        /// Thực thi câu lệnh SELECT và trả về DataReader
        /// </summary>
        public static OdbcDataReader ExecuteReader(string sqlCommand)
        {
            try
            {
                if (connection == null || connection.State == ConnectionState.Closed)
                {
                    ConnectDB();
                }
                
                dml = new OdbcCommand(sqlCommand, connection);
                dr = dml.ExecuteReader();
                return dr;
            }
            catch (OdbcException odbcEx)
            {
                HandleOdbcException(odbcEx, "Đọc dữ liệu");
                throw;
            }
        }

        /// <summary>
        /// Xử lý ODBC Exception chi tiết
        /// </summary>
        private static void HandleOdbcException(OdbcException odbcEx, string operation)
        {
            string errorMessage = $"[{Constants.ERR_DB_QUERY}] Lỗi ODBC khi {operation}:\n\n";
            
            foreach (OdbcError error in odbcEx.Errors)
            {
                errorMessage += $"Thông báo: {error.Message}\n";
                errorMessage += $"Mã lỗi: {error.NativeError}\n";
                errorMessage += $"Nguồn: {error.Source}\n";
                errorMessage += $"SQL State: {error.SQLState}\n";
                errorMessage += new string('-', 50) + "\n";
            }
            
            MessageBox.Show(errorMessage, Constants.DbMessages.QUERY_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
