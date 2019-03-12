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

        private void buttonAddRow_Click(object sender, EventArgs e) {
            ManualAddCar();
        }

        public void ManualAddCar() {
            connect.Open();

            string commandString = String.Format("INSERT INTO Car " +
                "(RegistrationMark, Make, Model, TrimLevel, Mileage, Colour, Year, Price, Url, Location, DateAdded, MotExpiry) " +
                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}')",
                textBox_Reg.Text, textBox_Make.Text, textBox_Model.Text, textBox_Trim.Text, textBox_Mileage.Text, textBox_Colour.Text,
                textBox_Year.Text, textBox_Price.Text, textBox_URL.Text, textBox_Location.Text, dateTimePicker_DateAdded.Text, textBox_MOT.Text
                );
            textBoxTest.AppendText(commandString);
            SqlCommand adder = new SqlCommand(commandString, connect);
            adder.ExecuteNonQuery();
            connect.Close();

            
        }

    }
}
