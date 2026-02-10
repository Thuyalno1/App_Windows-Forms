using System.Data.Odbc;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace path_2
{
    public partial class DangKy : Form
    {
        public DangKy()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Validation
            if (!ValidateInput())
                return;

            try
            {
                RegisterUser();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đăng ký: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            // Kiểm tra username
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập username!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return false;
            }

            if (txtUsername.Text.Length < 3)
            {
                MessageBox.Show("Username phải có ít nhất 3 ký tự!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return false;
            }

            // Kiểm tra email
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Email không hợp lệ!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            // Kiểm tra password
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập password!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Password phải có ít nhất 6 ký tự!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            // Kiểm tra confirm password
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Password không khớp!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private void RegisterUser()
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            // Tạo salt và hash password
            string salt = PasswordHelper.GenerateSalt();
            string passwordHash = PasswordHelper.HashPassword(password, salt);

            using (OdbcConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // Kiểm tra username đã tồn tại chưa
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = ? OR Email = ?";
                using (OdbcCommand checkCmd = new OdbcCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.Add("?", OdbcType.VarChar).Value = username;
                    checkCmd.Parameters.Add("?", OdbcType.VarChar).Value = email;

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        MessageBox.Show("Username hoặc Email đã tồn tại!", "Thông báo", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Insert user mới
                string insertQuery = @"INSERT INTO Users (Username, Email, PasswordHash, Salt, CreatedAt) 
                                      VALUES (?, ?, ?, ?, ?)";

                using (OdbcCommand insertCmd = new OdbcCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.Add("?", OdbcType.VarChar).Value = username;
                    insertCmd.Parameters.Add("?", OdbcType.VarChar).Value = email;
                    insertCmd.Parameters.Add("?", OdbcType.VarChar).Value = passwordHash;
                    insertCmd.Parameters.Add("?", OdbcType.VarChar).Value = salt;
                    insertCmd.Parameters.Add("?", OdbcType.DateTime).Value = DateTime.Now;

                    int rowsAffected = insertCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Đăng ký thành công!", "Thành công", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Clear form
                        ClearForm();

                        // Có thể mở form login hoặc đóng form register
                        // DangNhap dangNhap = new DangNhap();
                        // dangNhap.Show();
                        // this.Close();
                    }
                }
            }
        }

        private void ClearForm()
        {
            txtUsername.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            txtConfirmPassword.Clear();
            txtUsername.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void lblLogin_Click(object sender, EventArgs e)
        {
            // Mở form login
            DangNhap dangNhap = new DangNhap();
            dangNhap.Show();
            this.Hide();
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
