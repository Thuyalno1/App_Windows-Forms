using System.Data.Odbc;
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
            OdbcConnection con = new OdbcConnection(conString);
            con.Open();

            string query = "Select * from student"; // d�ng b?ng b?n d� t?o
            OdbcCommand cmd = new OdbcCommand(query, con);

            OdbcDataReader reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);

            dataGridView1.DataSource = dt; // gi? s? KH�NG l?i n?a
        }
    }
}

