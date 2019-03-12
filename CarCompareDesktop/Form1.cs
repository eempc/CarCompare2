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
            StartMethod();
        }

        TextBox[] textBoxes;
        public void StartMethod() {
            textBoxes = new TextBox[] {
                textBox_ID, textBox_Reg, textBox_Make, textBox_Model, textBox_Trim, textBox_Mileage, textBox_Colour,
                textBox_Year, textBox_Price, textBox_URL, textBox_Location, textBox_MOT
             };
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
            string commandString = string.Format("INSERT INTO Car " +
                "(RegistrationMark, Make, Model, TrimLevel, Mileage, Colour, Year, Price, Url, Location, DateAdded, MotExpiry) " +
                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}')",
                textBox_Reg.Text, textBox_Make.Text, textBox_Model.Text, textBox_Trim.Text, textBox_Mileage.Text, textBox_Colour.Text,
                textBox_Year.Text, textBox_Price.Text, textBox_URL.Text, textBox_Location.Text, dateTimePicker_DateAdded.Text, textBox_MOT.Text
            );

            connect.Open();
            SqlCommand adder = new SqlCommand(commandString, connect);
            adder.ExecuteNonQuery();
            connect.Close();           
        }

        public void UpdateRow() {

        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e) {
            //textBox_ID.Text = listView1.SelectedItems[0].SubItems[0].Text;
            for (int i = 0; i < textBoxes.Length; i++) {
                if (i != textBoxes.Length-1) {
                    textBoxes[i].Text = listView1.SelectedItems[0].SubItems[i].Text;
                } else {
                    textBoxes[i].Text = listView1.SelectedItems[0].SubItems[i+1].Text;
                }
            }
        }
    }
}
