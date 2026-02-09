using System.Data.Odbc;
using System;
using System.Windows.Forms;

namespace path_2
{
    public partial class DangNhap : Form
    {
        public DangNhap()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Validation
            if (!ValidateInput())
                return;

            try
            {
                LoginUser();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Constants.AuthMessages.LOGIN_FAILED}\n{ex.Message}", 
                    $"Lỗi {Constants.ERR_LOGIN_FAILED}", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show(Constants.ValidationMessages.RequiredField("Tên đăng nhập"), 
                    "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show(Constants.ValidationMessages.RequiredField("Mật khẩu"), 
                    "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            return true;
        }

        private void LoginUser()
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            using (OdbcConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // Lấy thông tin user từ database (bao gồm Role)
                string query = "SELECT Id, Username, Email, PasswordHash, Salt, Role FROM Users WHERE Username = ?";

                using (OdbcCommand cmd = new OdbcCommand(query, conn))
                {
                    cmd.Parameters.Add("?", OdbcType.VarChar).Value = username;

                    using (OdbcDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // ODBC yêu cầu dùng column index thay vì column name
                            // SELECT Id, Username, Email, PasswordHash, Salt, Role FROM Users
                            int userId = reader.GetInt32(0);           // Id
                            string storedHash = reader.GetString(3);   // PasswordHash
                            string storedSalt = reader.GetString(4);   // Salt
                            string email = reader.GetString(2);        // Email
                            string role = reader.GetString(5);         // Role

                            // Verify password
                            if (PasswordHelper.VerifyPassword(password, storedHash, storedSalt))
                            {
                                reader.Close();

                                // Cập nhật LastLogin
                                UpdateLastLogin(conn, userId);

                                //  LƯU THÔNG TIN USER VÀO CurrentUser
                                CurrentUser.Id = userId;
                                CurrentUser.Username = username;
                                CurrentUser.Email = email;
                                CurrentUser.Role = role;

                                MessageBox.Show($"{Constants.AuthMessages.LOGIN_SUCCESS}\n\nChào mừng {username} ({email})\nVai trò: {role}", 
                                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                //  MỞ MAIN DASHBOARD FORM
                                MainDashboardForm dashboard = new MainDashboardForm();
                                dashboard.Show();
                                this.Hide();  // Ẩn DangNhap

                            }
                            else
                            {
                                MessageBox.Show(Constants.AuthMessages.LOGIN_FAILED, 
                                    $"Lỗi {Constants.ERR_LOGIN_FAILED}", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtPassword.Clear();
                                txtPassword.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show(Constants.AuthMessages.LOGIN_FAILED, 
                                $"Lỗi {Constants.ERR_LOGIN_FAILED}", 
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtUsername.Focus();
                        }
                    }
                }
            }
        }

        private void UpdateLastLogin(OdbcConnection conn, int userId)
        {
            string updateQuery = "UPDATE Users SET LastLogin = ? WHERE Id = ?";
            using (OdbcCommand updateCmd = new OdbcCommand(updateQuery, conn))
            {
                updateCmd.Parameters.Add("?", OdbcType.DateTime).Value = DateTime.Now;
                updateCmd.Parameters.Add("?", OdbcType.Int).Value = userId;
                updateCmd.ExecuteNonQuery();
            }
        }

        private void ClearForm()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtUsername.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblRegister_Click(object sender, EventArgs e)
        {
            // Mở form register
            DangKy dangKy = new DangKy();
            dangKy.Show();
            this.Hide();
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            // Toggle hiển thị password
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '●';
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            // Event handler được Designer tự động thêm
            // Không cần xử lý gì ở đây
        }
    }
}
