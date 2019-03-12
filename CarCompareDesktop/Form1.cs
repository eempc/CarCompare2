using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarCompareDesktop {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Evie\CarCompareContext-d1a204cc-6cb2-4983-b6ca-8e135f56615c.mdf;Integrated Security=True");

        private void buttonDisplayAll_Click(object sender, EventArgs e) {
            DisplayAll();
        }

        // Non-async method of querying database
        public void DisplayAll() {
            listView1.Items.Clear();

            connect.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM Car", connect);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read()) {
                //textBoxTest.AppendText(reader.GetValue(1) + "\r\n");
                ListViewItem newItem = new ListViewItem(reader.GetValue(0).ToString());
                for (int i = 1; i < 13; i++) {
                    newItem.SubItems.Add(reader.GetValue(i).ToString());
                }
                listView1.Items.Add(newItem);
            }
            reader.Close();
            connect.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();
    }
}
