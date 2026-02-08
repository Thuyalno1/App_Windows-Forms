using System;
using System.Windows.Forms;

namespace path_2
{
    public partial class MainDashboardForm : Form
    {
        public MainDashboardForm()
        {
            InitializeComponent();
            ConfigureForRole();
        }

        private void ConfigureForRole()
        {
            // Hiển thị welcome message
            lblWelcome.Text = $"Xin chào: {CurrentUser.Username}";
            lblRole.Text = $"Vai trò: {CurrentUser.Role}";

            if (CurrentUser.IsAdmin)
            {
                // Admin thấy đầy đủ menu
                btnUserManagement.Visible = true;
                btnJobManagement.Text = "📋 Quản Lý Công Việc\n(Giao việc cho nhân viên)";
            }
            else
            {
                // User không thấy menu Quản lý nhân viên
                btnUserManagement.Visible = false;
                btnJobManagement.Text = "📋 Công Việc Của Tôi\n(Xem việc được giao)";
            }
        }

        private void btnUserManagement_Click(object sender, EventArgs e)
        {
            // Chỉ Admin mới vào được
            if (!CurrentUser.IsAdmin)
            {
                MessageBox.Show("Bạn không có quyền truy cập!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UserManagementForm userForm = new UserManagementForm();
            userForm.Show();
            this.Hide();
        }

        private void btnJobManagement_Click(object sender, EventArgs e)
        {
            QuanLyCongViec jobForm = new QuanLyCongViec();
            jobForm.Show();
            this.Hide();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn đăng xuất?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                CurrentUser.Clear();

                DangNhap dangNhap = new DangNhap();
                dangNhap.Show();
                this.Close();
            }
        }
    }
}
