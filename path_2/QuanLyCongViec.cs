using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace path_2
{
    public partial class QuanLyCongViec : Form
    {
        private int selectedJobId = -1;

        public QuanLyCongViec()
        {
            InitializeComponent();
            ConfigureForRole();
            LoadUsers();  // Load danh sách users cho ComboBox (nếu là Admin)
            LoadJobs();
        }

        private void ConfigureForRole()
        {
            lblWelcome.Text = $"Xin chào: {CurrentUser.Username} ({CurrentUser.Role})";

            if (CurrentUser.IsAdmin)
            {
                // Admin: Hiển thị ComboBox giao việc
                lblAssignTo.Visible = true;
                cmbAssignTo.Visible = true;
                
                // Admin có thể thêm/sửa/xóa
                btnAdd.Enabled = true;
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                // User: Ẩn ComboBox giao việc
                lblAssignTo.Visible = false;
                cmbAssignTo.Visible = false;
                
                // User không được thêm/sửa/xóa
                btnAdd.Enabled = false;
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void LoadUsers()
        {
            if (!CurrentUser.IsAdmin)
                return;  // User không cần load danh sách

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Lấy danh sách Users (không bao gồm Admin)
                    string query = "SELECT Id, Username FROM Users WHERE Role = 'User' ORDER BY Username";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            cmbAssignTo.DisplayMember = "Username";
                            cmbAssignTo.ValueMember = "Id";
                            cmbAssignTo.DataSource = dt;
                            cmbAssignTo.SelectedIndex = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi load danh sách nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadJobs()
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query;

                    if (CurrentUser.IsAdmin)
                    {
                        // Admin thấy TẤT CẢ jobs
                        query = @"SELECT j.Id, j.Name, j.Difficulty, j.CreatedAt, 
                                         creator.Username AS 'Người tạo',
                                         IFNULL(assignee.Username, '(Chưa giao)') AS 'Người nhận'
                                  FROM Jobs j
                                  LEFT JOIN Users creator ON j.CreatedBy = creator.Id
                                  LEFT JOIN Users assignee ON j.AssignedTo = assignee.Id
                                  ORDER BY j.CreatedAt DESC";
                    }
                    else
                    {
                        // User chỉ thấy jobs được giao cho MÌNH
                        query = @"SELECT j.Id, j.Name, j.Difficulty, j.CreatedAt,
                                         creator.Username AS 'Người giao'
                                  FROM Jobs j
                                  LEFT JOIN Users creator ON j.CreatedBy = creator.Id
                                  WHERE j.AssignedTo = @userId
                                  ORDER BY j.CreatedAt DESC";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (!CurrentUser.IsAdmin)
                        {
                            cmd.Parameters.AddWithValue("@userId", CurrentUser.Id);
                        }

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvJobs.DataSource = dt;

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
            if (dgvJobs.Columns.Count > 0)
            {
                dgvJobs.Columns["Id"].HeaderText = "ID";
                dgvJobs.Columns["Id"].Width = 50;

                dgvJobs.Columns["Name"].HeaderText = "Tên Công Việc";
                dgvJobs.Columns["Name"].Width = 200;

                dgvJobs.Columns["Difficulty"].HeaderText = "Độ Khó";
                dgvJobs.Columns["Difficulty"].Width = 100;

                dgvJobs.Columns["CreatedAt"].HeaderText = "Ngày Tạo";
                dgvJobs.Columns["CreatedAt"].Width = 130;
                dgvJobs.Columns["CreatedAt"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

                // Cột khác nhau cho Admin và User
                if (CurrentUser.IsAdmin)
                {
                    dgvJobs.Columns["Người tạo"].Width = 120;
                    dgvJobs.Columns["Người nhận"].Width = 120;
                }
                else
                {
                    dgvJobs.Columns["Người giao"].Width = 120;
                }

                dgvJobs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvJobs.MultiSelect = false;
                dgvJobs.ReadOnly = true;
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtJobName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên công việc!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtJobName.Focus();
                return false;
            }

            if (cmbDifficulty.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn độ khó!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDifficulty.Focus();
                return false;
            }

            // Admin PHẢI chọn người được giao việc
            if (CurrentUser.IsAdmin && cmbAssignTo.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn nhân viên để giao việc!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbAssignTo.Focus();
                return false;
            }

            return true;
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

                    string query = @"INSERT INTO Jobs (UserId, CreatedBy, AssignedTo, Name, Difficulty, CreatedAt) 
                                   VALUES (@userId, @createdBy, @assignedTo, @name, @difficulty, @createdAt)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", CurrentUser.Id);  // Thêm UserId
                        cmd.Parameters.AddWithValue("@createdBy", CurrentUser.Id);
                        cmd.Parameters.AddWithValue("@assignedTo", cmbAssignTo.SelectedValue);
                        cmd.Parameters.AddWithValue("@name", txtJobName.Text.Trim());
                        cmd.Parameters.AddWithValue("@difficulty", cmbDifficulty.Text);
                        cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Giao việc thành công!", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            ClearForm();
                            LoadJobs();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm công việc: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedJobId == -1)
            {
                MessageBox.Show("Vui lòng chọn công việc cần sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInput())
                return;

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE Jobs 
                                   SET Name = @name, Difficulty = @difficulty, AssignedTo = @assignedTo 
                                   WHERE Id = @jobId AND CreatedBy = @userId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@jobId", selectedJobId);
                        cmd.Parameters.AddWithValue("@userId", CurrentUser.Id);
                        cmd.Parameters.AddWithValue("@name", txtJobName.Text.Trim());
                        cmd.Parameters.AddWithValue("@difficulty", cmbDifficulty.Text);
                        cmd.Parameters.AddWithValue("@assignedTo", cmbAssignTo.SelectedValue);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Cập nhật công việc thành công!", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            ClearForm();
                            LoadJobs();
                            selectedJobId = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật công việc: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedJobId == -1)
            {
                MessageBox.Show("Vui lòng chọn công việc cần xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn xóa công việc này?",
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

                        string query = "DELETE FROM Jobs WHERE Id = @jobId AND CreatedBy = @userId";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@jobId", selectedJobId);
                            cmd.Parameters.AddWithValue("@userId", CurrentUser.Id);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Xóa công việc thành công!", "Thành công",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                                ClearForm();
                                LoadJobs();
                                selectedJobId = -1;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi xóa công việc: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadJobs();
            ClearForm();
            selectedJobId = -1;
        }

        private void dgvJobs_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvJobs.SelectedRows.Count > 0 && CurrentUser.IsAdmin)
            {
                DataGridViewRow row = dgvJobs.SelectedRows[0];

                selectedJobId = Convert.ToInt32(row.Cells["Id"].Value);
                txtJobName.Text = row.Cells["Name"].Value.ToString();
                cmbDifficulty.Text = row.Cells["Difficulty"].Value.ToString();
                
                // Load assigned user
                string assignedTo = row.Cells["Người nhận"].Value.ToString();
                if (assignedTo != "(Chưa giao)")
                {
                    cmbAssignTo.Text = assignedTo;
                }
            }
        }

        private void ClearForm()
        {
            txtJobName.Clear();
            cmbDifficulty.SelectedIndex = -1;
            if (CurrentUser.IsAdmin)
            {
                cmbAssignTo.SelectedIndex = -1;
            }
            selectedJobId = -1;
            txtJobName.Focus();
        }

        private void btnBackToDashboard_Click(object sender, EventArgs e)
        {
            MainDashboardForm dashboard = new MainDashboardForm();
            dashboard.Show();
            this.Close();
        }
    }
}
