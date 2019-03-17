﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;


namespace CarCompareDesktop {
    public partial class MainWindowsForm : Form {
        public MainWindowsForm() {
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

            buttonUpdateCar.Enabled = false;
        }
        
        private async void button1_Click(object sender, EventArgs e) {
            string html = await WebScraper.GetHtmlViaHttpClientAsync(@"http://eempc.github.io/index.html");
            textBoxTest.AppendText(html);
        }

        private void buttonDisplayAll_Click(object sender, EventArgs e) {
            DisplayAll();
        }

        // Non-async method of querying database. Loops through the properties of an object via reflections, then put data into a ListView
        public async void DisplayAll() {
            listView1.Items.Clear();

            List<SqlCar> myCars = await SqlCar.ReadDatabaseAsync("SELECT * FROM Car");

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

        private void buttonCreateCar_Click(object sender, EventArgs e) {
            CreateNewCar();
        }

        // String format/interpolation method (bad because of SQL Injection)
        public async void CreateNewCar() {
            SqlCar newCar = new SqlCar();
            newCar.registration = textBox_Reg.Text;
            newCar.make = textBox_Make.Text;
            newCar.model = textBox_Model.Text;
            newCar.trim = textBox_Trim.Text;
            newCar.mileage = int.Parse(textBox_Mileage.Text);
            newCar.colour = textBox_Colour.Text;
            newCar.year = int.Parse(textBox_Year.Text);
            newCar.price = decimal.Parse(textBox_Price.Text);
            newCar.url = textBox_URL.Text;
            newCar.location = textBox_Location.Text;
            newCar.dateAdded = DateTime.Today;
            newCar.mot = int.Parse(textBox_MOT.Text);

            string commandString = String.Format("INSERT INTO Car " +
                "(RegistrationMark, Make, Model, TrimLevel, Mileage, Colour, Year, Price, Url, Location, DateAdded, MotExpiry) " +
                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}')", 
                textBox_Reg.Text, textBox_Make.Text, textBox_Model.Text, textBox_Trim.Text, textBox_Mileage.Text, 
                textBox_Colour.Text, textBox_Year.Text, textBox_Price.Text, textBox_URL.Text, textBox_Location.Text, 
                dateTimePicker_DateAdded.Text, textBox_MOT.Text);

            await SqlCar.CreateDatabaseEntryAsync(newCar);         
        }
        
        // UPDATING AN ENTRY
        // Right click Edit on a listview item ID number to populate the textbox fields
        private void editToolStripMenuItem1_Click(object sender, EventArgs e) {
            buttonUpdateCar.Enabled = true;
            for (int i = 0; i < textBoxes.Length; i++) {
                if (i != textBoxes.Length-1) {
                    textBoxes[i].Text = listView1.SelectedItems[0].SubItems[i].Text;
                } else {
                    textBoxes[i].Text = listView1.SelectedItems[0].SubItems[i+1].Text;
                }
            }
        }

        // Then click the button to activate the method
        private void buttonUpdateCar_Click(object sender, EventArgs e) {
            UpdateCar();
        }

        // The method collects the text from the textboxes and sends it to be processed on the SqlCar class
        public void UpdateCar() {
            string[] textBoxStrings = { textBox_ID.Text, textBox_Reg.Text, textBox_Make.Text, textBox_Model.Text, textBox_Trim.Text, textBox_Mileage.Text,
                textBox_Colour.Text, textBox_Year.Text, textBox_Price.Text, textBox_URL.Text, textBox_Location.Text,
                dateTimePicker_DateAdded.Text, textBox_MOT.Text };

            SqlCar.UpdateDatabaseEntry(textBoxStrings);
        }

        // Right click to delete the entry in listview
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
                DeleteCar();                     
        }

        public void DeleteCar() {
        DialogResult result = MessageBox.Show(
            "Are you sure you want to delete this entry?",
            "Important Question",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

            if (result == DialogResult.Yes) {
                SqlCar.DeleteDatabaseEntry(listView1.SelectedItems[0].SubItems[0].Text);
            }
        }

        public void DeleteByCriteria() {

        }
    }
}
