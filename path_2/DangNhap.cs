using MySql.Data.MySqlClient;
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
                MessageBox.Show($"Lỗi đăng nhập: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập username!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập password!", "Thông báo", 
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

            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // Lấy thông tin user từ database (bao gồm Role)
                string query = "SELECT Id, Username, Email, PasswordHash, Salt, Role FROM Users WHERE Username = @username";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = reader.GetInt32("Id");
                            string storedHash = reader.GetString("PasswordHash");
                            string storedSalt = reader.GetString("Salt");
                            string email = reader.GetString("Email");
                            string role = reader.GetString("Role");

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

                                MessageBox.Show($"Đăng nhập thành công!\n\nChào mừng {username} ({email})\nVai trò: {role}", 
                                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                //  MỞ MAIN DASHBOARD FORM
                                MainDashboardForm dashboard = new MainDashboardForm();
                                dashboard.Show();
                                this.Hide();  // Ẩn DangNhap

                            }
                            else
                            {
                                MessageBox.Show("Password không đúng!", "Thông báo", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtPassword.Clear();
                                txtPassword.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Username không tồn tại!", "Thông báo", 
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtUsername.Focus();
                        }
                    }
                }
            }
        }

        private void UpdateLastLogin(MySqlConnection conn, int userId)
        {
            string updateQuery = "UPDATE Users SET LastLogin = @lastLogin WHERE Id = @userId";
            using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
            {
                updateCmd.Parameters.AddWithValue("@lastLogin", DateTime.Now);
                updateCmd.Parameters.AddWithValue("@userId", userId);
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
