using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
        }

        private void buttonDisplayAll_Click(object sender, EventArgs e) {
            DisplayAll();
        }

        // Non-async method of querying database. Loops through the properties of an object via reflections
        public void DisplayAll() {
            listView1.Items.Clear();

            List<SqlCar> myCars = SqlCar.AccessSqlReader("SELECT * FROM Car");

            foreach (var car in myCars) {
                ListViewItem newItem = new ListViewItem(car.id.ToString());
                foreach (PropertyInfo prop in car.GetType().GetProperties()) {
                    if (prop.Name != "id")
                        newItem.SubItems.Add(prop.GetValue(car, null).ToString());
                }
                listView1.Items.Add(newItem);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void buttonAddRow_Click(object sender, EventArgs e) {
            ManualAddCar();
        }

        // String format/interpolation method (bad because of SQL Injection)
        public void ManualAddCar() {
            string commandString = String.Format("INSERT INTO Car " +
                "(RegistrationMark, Make, Model, TrimLevel, Mileage, Colour, Year, Price, Url, Location, DateAdded, MotExpiry) " +
                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}')", 
                textBox_Reg.Text, textBox_Make.Text, textBox_Model.Text, textBox_Trim.Text, textBox_Mileage.Text, 
                textBox_Colour.Text, textBox_Year.Text, textBox_Price.Text, textBox_URL.Text, textBox_Location.Text, 
                dateTimePicker_DateAdded.Text, textBox_MOT.Text);

            SqlCar.ExecuteNonQuery(commandString);         
        }
        
        // UPDATING 
        // Right click Edit on a listview item ID number to populate the textbox fields
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

        // Then click the button to activate the method
        private void button_EditRow_Click(object sender, EventArgs e) {
            UpdateRow();
        }

        // The method collects the text from the textboxes and sends it to be processed on the SqlCar class
        public void UpdateRow() {
            string[] textBoxStrings = { textBox_ID.Text, textBox_Reg.Text, textBox_Make.Text, textBox_Model.Text, textBox_Trim.Text, textBox_Mileage.Text,
                textBox_Colour.Text, textBox_Year.Text, textBox_Price.Text, textBox_URL.Text, textBox_Location.Text,
                dateTimePicker_DateAdded.Text, textBox_MOT.Text };

            SqlCar.UpdateDatabaseEntry(textBoxStrings);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this entry?", 
                "Important Question", 
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes) {
                DeleteRow();
            }            
        }

        public void DeleteRow() {
            SqlCar.ExecuteNonQuery(String.Format("DELETE FROM Car WHERE Id = '{0}'", listView1.SelectedItems[0].SubItems[0].Text));
        }
    }
}
