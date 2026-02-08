using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace path_2
{
    public partial class UserManagementForm : Form
    {
        private int selectedUserId = -1;

        public UserManagementForm()
        {
            InitializeComponent();
            
            // Kiểm tra quyền Admin
            if (!CurrentUser.IsAdmin)
            {
                MessageBox.Show("Bạn không có quyền truy cập!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Lấy tất cả users
                    string query = @"SELECT Id, Username, Email, Role, CreatedAt 
                                   FROM Users 
                                   ORDER BY CreatedAt DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvUsers.DataSource = dt;

                            FormatDataGridView();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi load dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDataGridView()
        {
            if (dgvUsers.Columns.Count > 0)
            {
                dgvUsers.Columns["Id"].HeaderText = "ID";
                dgvUsers.Columns["Id"].Width = 50;

                dgvUsers.Columns["Username"].HeaderText = "Tên đăng nhập";
                dgvUsers.Columns["Username"].Width = 150;

                dgvUsers.Columns["Email"].HeaderText = "Email";
                dgvUsers.Columns["Email"].Width = 200;

                dgvUsers.Columns["Role"].HeaderText = "Vai trò";
                dgvUsers.Columns["Role"].Width = 100;

                dgvUsers.Columns["CreatedAt"].HeaderText = "Ngày tạo";
                dgvUsers.Columns["CreatedAt"].Width = 150;
                dgvUsers.Columns["CreatedAt"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

                dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvUsers.MultiSelect = false;
                dgvUsers.ReadOnly = true;
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

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            // Validate email format
            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Email không hợp lệ!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập password!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            if (cmbRole.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn vai trò!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRole.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Kiểm tra trùng username
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("Username đã tồn tại!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Tạo user mới
                    string salt = PasswordHelper.GenerateSalt();
                    string passwordHash = PasswordHelper.HashPassword(txtPassword.Text, salt);

                    string query = @"INSERT INTO Users (Username, Email, PasswordHash, Salt, Role, CreatedAt) 
                                   VALUES (@username, @email, @passwordHash, @salt, @role, @createdAt)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                        cmd.Parameters.AddWithValue("@salt", salt);
                        cmd.Parameters.AddWithValue("@role", cmbRole.Text);
                        cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Thêm nhân viên thành công!", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            ClearForm();
                            LoadUsers();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedUserId == -1)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Không cho edit Admin account
            if (selectedUserId == 1)
            {
                MessageBox.Show("Không thể sửa tài khoản Admin chính!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE Users 
                                   SET Username = @username, Email = @email, Role = @role 
                                   WHERE Id = @userId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", selectedUserId);
                        cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@role", cmbRole.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Cập nhật thông tin thành công!", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            ClearForm();
                            LoadUsers();
                            selectedUserId = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedUserId == -1)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Không cho xóa chính mình
            if (selectedUserId == CurrentUser.Id)
            {
                MessageBox.Show("Không thể xóa tài khoản của chính bạn!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Không cho xóa Admin account chính
            if (selectedUserId == 1)
            {
                MessageBox.Show("Không thể xóa tài khoản Admin chính!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn xóa nhân viên này?\n(Tất cả công việc liên quan sẽ bị xóa)",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();

                        string query = "DELETE FROM Users WHERE Id = @userId";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@userId", selectedUserId);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Xóa nhân viên thành công!", "Thành công",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                                ClearForm();
                                LoadUsers();
                                selectedUserId = -1;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi xóa nhân viên: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadUsers();
            ClearForm();
            selectedUserId = -1;
        }

        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvUsers.SelectedRows[0];

                selectedUserId = Convert.ToInt32(row.Cells["Id"].Value);
                txtUsername.Text = row.Cells["Username"].Value.ToString();
                txtEmail.Text = row.Cells["Email"].Value.ToString();
                cmbRole.Text = row.Cells["Role"].Value.ToString();
                txtPassword.Text = "";  // Không hiển thị password
            }
        }

        private void ClearForm()
        {
            txtUsername.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            cmbRole.SelectedIndex = -1;
            selectedUserId = -1;
            txtUsername.Focus();
        }

        private void btnBackToDashboard_Click(object sender, EventArgs e)
        {
            MainDashboardForm dashboard = new MainDashboardForm();
            dashboard.Show();
            this.Close();
        }
    }
}
