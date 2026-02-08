using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace path_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getData();
        }

        public void getData()
        {
            string conString = "server=localhost;uid=root;pwd=thuymv;database=path_1;";
            MySqlConnection con = new MySqlConnection(conString);
            con.Open();

            string query = "Select * from student"; // đúng bảng bạn đã tạo
            MySqlCommand cmd = new MySqlCommand(query, con);

            MySqlDataReader reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);

            dataGridView1.DataSource = dt; // giờ sẽ KHÔNG lỗi nữa
        }
    }
}
