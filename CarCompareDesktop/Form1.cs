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

// Move all SQL stuff to a new class later, i.e. aim is to remove System.Data.SqlClient. 
// All this repeating connect.Open() stuff is readable but not clean
// ListView1's headings should be programmatically added and not manually entered into the designer
// SQL column DateAdded should be second column or last column

namespace CarCompareDesktop {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            StartMethod();
            ColumnsNames();
        }

        TextBox[] textBoxes;
        List<string> columnNames = new List<string>();
        

        public void StartMethod() {
            textBoxes = new TextBox[] {
                textBox_ID, textBox_Reg, textBox_Make, textBox_Model, textBox_Trim, textBox_Mileage, textBox_Colour,
                textBox_Year, textBox_Price, textBox_URL, textBox_Location, textBox_MOT
             };

            button_EditRow.Enabled = false;
        }
        
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Evie\CarCompareContext-d1a204cc-6cb2-4983-b6ca-8e135f56615c.mdf;Integrated Security=True");

        private void button1_Click(object sender, EventArgs e) {
            List<string> myList = SqlCar.GetColumnNames();
            foreach (string item in myList) {
                textBoxTest.AppendText(item + Environment.NewLine);
            }
            List<SqlCar> myCars = SqlCar.AccessSqlReader("SELECT * FROM Car");
            foreach (var car in myCars) {
                textBoxTest.AppendText(car.registration + "\r\n");
            }
        }



        public void ColumnsNames() {
            connect.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM Car WHERE 1 = 2", connect);            
            SqlDataReader reader = command.ExecuteReader();

            for (int i = 0; i < reader.FieldCount; i++)
                columnNames.Add(reader.GetName(i));

            reader.Close();
            connect.Close();                   
        }

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

        // String interpolation method
        public void ManualAddCar() {
            string commandString = String.Format("INSERT INTO Car " +
                "(RegistrationMark, Make, Model, TrimLevel, Mileage, Colour, Year, Price, Url, Location, DateAdded, MotExpiry) " +
                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}')", 
                textBox_Reg.Text, textBox_Make.Text, textBox_Model.Text, textBox_Trim.Text, textBox_Mileage.Text, 
                textBox_Colour.Text, textBox_Year.Text, textBox_Price.Text, textBox_URL.Text, textBox_Location.Text, 
                dateTimePicker_DateAdded.Text, textBox_MOT.Text);

            connect.Open();
            SqlCommand adder = new SqlCommand(commandString, connect);
            adder.ExecuteNonQuery();
            connect.Close();           
        }

        // Parameters Add With Value
        public void UpdateRow() {

            List<string> fieldIdentifiers = new List<string>();
            fieldIdentifiers.Add("@_id");

            string cmdText = "UPDATE Car SET ";

            for (int i = 1; i < columnNames.Count; i++) {
                if (i != columnNames.Count - 2) {
                    // I.e. not the Date Added field at n-2 (i wish it were last on the last)
                    // Static columns should be at the start, e.g. Id and this DateAdded
                    cmdText += columnNames[i] + " = @_" + columnNames[i] + ", ";
                    fieldIdentifiers.Add("@_" + columnNames[i]);
                }
            }

            cmdText = cmdText.Trim().TrimEnd(',');
            cmdText += " WHERE Id = @_id";
            //textBoxTest.AppendText(cmdText);
            connect.Open();
            SqlCommand updater = connect.CreateCommand();
            updater.CommandText = cmdText;

            for (int i = 0; i < textBoxes.Length; i++) {
                updater.Parameters.AddWithValue(fieldIdentifiers[i], textBoxes[i].Text);
                //textBoxTest.AppendText(fieldIdentifiers[i] + " " +  textBoxes[i].Text + "\r\n");
            }

            updater.ExecuteNonQuery();
            connect.Close();

            // Need to update Listview, this is why the data should've been downloaded and placed in a List<Car class>, to avoid querying the database for an updated list
        }

        // Right click Edit on an ID and it will populate the textboxes (not the date time picker). The textboxes were initialised into an array
        private void editToolStripMenuItem1_Click(object sender, EventArgs e) {
            button_EditRow.Enabled = true;
            for (int i = 0; i < textBoxes.Length; i++) {
                if (i != textBoxes.Length-1) {
                    textBoxes[i].Text = listView1.SelectedItems[0].SubItems[i].Text;
                } else {
                    textBoxes[i].Text = listView1.SelectedItems[0].SubItems[i+1].Text;
                }
            }
        }

        private void button_EditRow_Click(object sender, EventArgs e) {
            button_EditRow.Enabled = false;
            UpdateRow();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            DeleteRow();
        }

        public void DeleteRow() {
            connect.Open();
            SqlCommand deleter = new SqlCommand(String.Format("DELETE FROM Car WHERE Id = '{0}'",listView1.SelectedItems[0].SubItems[0].Text), connect);
            deleter.ExecuteNonQuery();
            connect.Close();
        }
    }
}
