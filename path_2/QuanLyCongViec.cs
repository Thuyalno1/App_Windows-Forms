using System.Data.Odbc;
using System;
using System.Data;
using System.Drawing;
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
                
                // User được CẬP NHẬT TIẾN ĐỘ
                grpProgress.Visible = true;
            }
        }

        private void LoadUsers()
        {
            if (!CurrentUser.IsAdmin)
                return;  // User không cần load danh sách

            try
            {
                using (OdbcConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Lấy danh sách Users (không bao gồm Admin)
                    string query = "SELECT Id, Username FROM Users WHERE Role = 'User' ORDER BY Username";

                    using (OdbcCommand cmd = new OdbcCommand(query, conn))
                    {
                        using (OdbcDataReader reader = cmd.ExecuteReader())
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
                using (OdbcConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query;

                    if (CurrentUser.IsAdmin)
                    {
                        // Admin thấy TẤT CẢ jobs
                        query = @"SELECT j.Id, j.Name, j.Difficulty, j.Progress, j.Status, j.LastUpdated, j.CreatedAt, 
                                         creator.Username AS 'Người tạo',
                                         IFNULL(assignee.Username, '(Chưa giao)') AS 'Người nhận',
                                         j.AssignedTo
                                  FROM Jobs j
                                  LEFT JOIN Users creator ON j.CreatedBy = creator.Id
                                  LEFT JOIN Users assignee ON j.AssignedTo = assignee.Id
                                  ORDER BY j.CreatedAt DESC";
                    }
                    else
                    {
                        // User chỉ thấy jobs được giao cho MÌNH
                        query = @"SELECT j.Id, j.Name, j.Difficulty, j.Progress, j.Status, j.LastUpdated, j.CreatedAt,
                                         creator.Username AS 'Người giao',
                                         j.AssignedTo
                                  FROM Jobs j
                                  LEFT JOIN Users creator ON j.CreatedBy = creator.Id
                                  WHERE j.AssignedTo = ?
                                  ORDER BY j.CreatedAt DESC";
                    }

                    using (OdbcCommand cmd = new OdbcCommand(query, conn))
                    {
                        if (!CurrentUser.IsAdmin)
                        {
                            cmd.Parameters.Add("?", OdbcType.Int).Value = CurrentUser.Id;
                        }

                        using (OdbcDataAdapter adapter = new OdbcDataAdapter(cmd))
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
                
                // Add Progress column formatting
                if (dgvJobs.Columns.Contains("Progress"))
                {
                    dgvJobs.Columns["Progress"].HeaderText = "Tiến độ (%)";
                    dgvJobs.Columns["Progress"].Width = 100;
                }
                
                if (dgvJobs.Columns.Contains("Status"))
                {
                    dgvJobs.Columns["Status"].HeaderText = "Trạng thái";
                    dgvJobs.Columns["Status"].Width = 120;
                }
                
                if (dgvJobs.Columns.Contains("LastUpdated"))
                {
                    dgvJobs.Columns["LastUpdated"].HeaderText = "Cập nhật lúc";
                    dgvJobs.Columns["LastUpdated"].Width = 130;
                    dgvJobs.Columns["LastUpdated"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                
                // Hide AssignedTo column (internal use only)
                if (dgvJobs.Columns.Contains("AssignedTo"))
                {
                    dgvJobs.Columns["AssignedTo"].Visible = false;
                }
                
                // Add cell formatting event for color-coded progress
                dgvJobs.CellFormatting -= DgvJobs_CellFormatting; // Remove existing to avoid duplicates
                dgvJobs.CellFormatting += DgvJobs_CellFormatting;
            }
        }
        
        private void DgvJobs_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Color-code Progress column
            if (dgvJobs.Columns[e.ColumnIndex].Name == "Progress" && e.Value != null && e.Value != DBNull.Value)
            {
                int progress = Convert.ToInt32(e.Value);
                e.Value = $"{progress}%";
                
                // Color coding: Red < 30%, Yellow 30-70%, Green > 70%
                if (progress < 30)
                    e.CellStyle.ForeColor = Color.Red;
                else if (progress < 70)
                    e.CellStyle.ForeColor = Color.Orange;
                else
                    e.CellStyle.ForeColor = Color.Green;
                    
                e.CellStyle.Font = new Font(dgvJobs.Font, FontStyle.Bold);
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
                using (OdbcConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO Jobs (UserId, CreatedBy, AssignedTo, Name, Difficulty, CreatedAt) 
                                   VALUES (?, ?, ?, ?, ?, ?)";

                    using (OdbcCommand cmd = new OdbcCommand(query, conn))
                    {
                        cmd.Parameters.Add("?", OdbcType.Int).Value = CurrentUser.Id;  // UserId
                        cmd.Parameters.Add("?", OdbcType.Int).Value = CurrentUser.Id;  // CreatedBy
                        cmd.Parameters.Add("?", OdbcType.Int).Value = cmbAssignTo.SelectedValue;
                        cmd.Parameters.Add("?", OdbcType.VarChar).Value = txtJobName.Text.Trim();
                        cmd.Parameters.Add("?", OdbcType.VarChar).Value = cmbDifficulty.Text;
                        cmd.Parameters.Add("?", OdbcType.DateTime).Value = DateTime.Now;

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
                using (OdbcConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE Jobs 
                                   SET Name = ?, Difficulty = ?, AssignedTo = ? 
                                   WHERE Id = ? AND CreatedBy = ?";

                    using (OdbcCommand cmd = new OdbcCommand(query, conn))
                    {
                        cmd.Parameters.Add("?", OdbcType.VarChar).Value = txtJobName.Text.Trim();  // Name
                        cmd.Parameters.Add("?", OdbcType.VarChar).Value = cmbDifficulty.Text;  // Difficulty
                        cmd.Parameters.Add("?", OdbcType.Int).Value = cmbAssignTo.SelectedValue;  // AssignedTo
                        cmd.Parameters.Add("?", OdbcType.Int).Value = selectedJobId;  // Id
                        cmd.Parameters.Add("?", OdbcType.Int).Value = CurrentUser.Id;  // CreatedBy

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
                    using (OdbcConnection conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();

                        string query = "DELETE FROM Jobs WHERE Id = ? AND CreatedBy = ?";

                        using (OdbcCommand cmd = new OdbcCommand(query, conn))
                        {
                            cmd.Parameters.Add("?", OdbcType.Int).Value = selectedJobId;
                            cmd.Parameters.Add("?", OdbcType.Int).Value = CurrentUser.Id;

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
            if (dgvJobs.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvJobs.SelectedRows[0];
                
                // Admin: Populate form fields for editing
                if (CurrentUser.IsAdmin)
                {
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
                
                // User: Populate progress controls
                if (!CurrentUser.IsAdmin && dgvJobs.Columns.Contains("Progress"))
                {
                    // Load current progress
                    if (row.Cells["Progress"].Value != null && row.Cells["Progress"].Value != DBNull.Value)
                    {
                        nudProgress.Value = Convert.ToInt32(row.Cells["Progress"].Value);
                    }
                    else
                    {
                        nudProgress.Value = 0;
                    }
                    
                    // Load current status
                    if (row.Cells["Status"].Value != null && row.Cells["Status"].Value != DBNull.Value)
                    {
                        txtStatus.Text = row.Cells["Status"].Value.ToString();
                    }
                    else
                    {
                        txtStatus.Text = "Pending";
                    }
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
        
        // ===== PROGRESS TRACKING METHODS =====
        
        private void nudProgress_ValueChanged(object sender, EventArgs e)
        {
            // Auto update status based on progress
            int progress = (int)nudProgress.Value;
            txtStatus.Text = AutoUpdateStatus(progress);
        }
        
        private string AutoUpdateStatus(int progress)
        {
            if (progress == 0) return "Pending";
            if (progress == 100) return "Completed";
            return "In Progress";
        }
        
        private void btnUpdateProgress_Click(object sender, EventArgs e)
        {
            if (!ValidateProgressUpdate())
                return;
            
            try
            {
                int jobId = Convert.ToInt32(dgvJobs.CurrentRow.Cells["Id"].Value);
                int progress = (int)nudProgress.Value;
                string status = txtStatus.Text;
                
                UpdateJobProgress(jobId, progress, status);
                
                MessageBox.Show("Cập nhật tiến độ thành công!", "Thành công", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                LoadJobs(); // Refresh
                
                // Clear progress inputs
                nudProgress.Value = 0;
                txtStatus.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật tiến độ: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private bool ValidateProgressUpdate()
        {
            if (dgvJobs.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn công việc để cập nhật tiến độ!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            
            // Chỉ user được assign mới update được
            if (!CurrentUser.IsAdmin) // User mode
            {
                int assignedTo = Convert.ToInt32(dgvJobs.CurrentRow.Cells["AssignedTo"].Value);
                if (assignedTo != CurrentUser.Id)
                {
                    MessageBox.Show("Bạn chỉ có thể cập nhật tiến độ công việc của mình!", 
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            
            return true;
        }
        
        private void UpdateJobProgress(int jobId, int progress, string status)
        {
            string query = @"UPDATE Jobs 
                            SET Progress = ?, Status = ?, LastUpdated = ? 
                            WHERE Id = ?";
            
            using (OdbcConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (OdbcCommand cmd = new OdbcCommand(query, conn))
                {
                    cmd.Parameters.Add("?", OdbcType.Int).Value = progress;
                    cmd.Parameters.Add("?", OdbcType.VarChar).Value = status;
                    cmd.Parameters.Add("?", OdbcType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("?", OdbcType.Int).Value = jobId;
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}


