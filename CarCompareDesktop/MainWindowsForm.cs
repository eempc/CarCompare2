using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;


namespace CarCompareDesktop
{
    public partial class MainWindowsForm : Form
    {
        public MainWindowsForm()
        {
            InitializeComponent();
            StartMethod();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        TextBox[] textBoxes;
        List<string> columnNames = new List<string>();
        List<SqlCar> myCars = new List<SqlCar>();

        public void StartMethod()
        {
            textBoxes = new TextBox[] {
                textBox_ID, textBox_Reg, textBox_Make, textBox_Model, textBox_Trim, textBox_Mileage, textBox_Colour,
                textBox_Year, textBox_Price, textBox_URL, textBox_Location, textBox_MOT
            };

            buttonUpdateCar.Enabled = false;

            // Auto suggestions for car makes, awesome
            var carMakes = new AutoCompleteStringCollection();
            carMakes.AddRange(Validations.carManufacturers);
            textBoxNewMake.AutoCompleteCustomSource = carMakes;
            textBoxNewMake.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBoxNewMake.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBoxNewMake.Visible = true;
        }

        // Test button
        private void button1_Click(object sender, EventArgs e)
        {
            //string html = await WebScraper.GetHtmlViaHttpClientAsync(@"http://eempc.github.io/index.html");
            //List<SqlCar> carList = SqlCar.ReadDatabaseByCondition();
            //foreach (SqlCar car in carList) {
            //    textBoxTest.AppendText(car.registration + " " + car.price + Environment.NewLine);
            //}    
            //textBoxTest.AppendText(Decimal.ToInt32(numericUpDownMOT.Value).ToString());
            //string fileText = WebScraper.GetHtmlViaWebClient(@"D:\Projects\CarCompareDesktop\CarCompareDesktop\Resources\TestCar2019-03-20.html");
            //string json = WebScraper.ExtractGumtreeJSON(fileText);
            string file = File.ReadAllText(@"D:\Projects\CarCompareDesktop\CarCompareDesktop\Resources\TestCar.html");
            SqlCar newCar = WebScraper.NewGumtreeCar(file);
            textBoxTest.AppendText(newCar.url);
        }

        private void buttonDisplayAll_Click(object sender, EventArgs e)
        {
            DisplayAll();
        }

        // Non-async method of querying database. Loops through the properties of an object via reflections, then put data into a ListView
        public async void DisplayAll()
        {
            myCars = await SqlCar.ReadDatabaseAsync("SELECT * FROM Car");
            PopulateListView(myCars);
        }

        public void PopulateListView(List<SqlCar> carList)
        {
            listView1.Items.Clear();
            foreach (var car in carList)
            {
                ListViewItem newItem = new ListViewItem(car.id.ToString());
                foreach (PropertyInfo prop in car.GetType().GetProperties())
                {
                    if (prop.Name != "id") newItem.SubItems.Add(prop.GetValue(car, null).ToString());
                }
                listView1.Items.Add(newItem);
            }
        }

        // CREATE
        private void buttonCreateCar_Click_1(object sender, EventArgs e)
        {
            CreateNewCar();
        }

        public async void CreateNewCar()
        {
            SqlCar newCar = new SqlCar();
            newCar.registration = textBoxNewReg.Text.ToUpper().Replace(" ", "");
            newCar.make = textBoxNewMake.Text;
            newCar.model = textBoxNewModel.Text;
            newCar.trim = textBoxNewTrim.Text;
            newCar.mileage = int.Parse(textBoxNewMileage.Text);
            newCar.colour = textBoxNewColour.Text;
            newCar.year = Decimal.ToInt32(numericUpDownYear.Value);
            newCar.price = decimal.Parse(textBoxNewPrice.Text);
            newCar.url = textBoxNewURL.Text;
            newCar.location = textBoxNewLocation.Text;
            newCar.dateAdded = DateTime.Today;
            newCar.mot = Decimal.ToInt32(numericUpDownMOT.Value);

            // TODO - Insert here: a check for duplicate cars via registration number before calling the DB

            await SqlCar.CreateDatabaseEntryAsync(newCar);
        }

        // UPDATING AN ENTRY
        // Right click Edit on a listview item ID number to populate the textbox fields
        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            buttonUpdateCar.Enabled = true;
            for (int i = 0; i < textBoxes.Length; i++)
            {
                if (i != textBoxes.Length - 1)
                {
                    textBoxes[i].Text = listView1.SelectedItems[0].SubItems[i].Text;
                }
                else
                {
                    textBoxes[i].Text = listView1.SelectedItems[0].SubItems[i + 1].Text;
                }
            }
        }

        // Then click the button to activate the method
        private void buttonUpdateCar_Click(object sender, EventArgs e)
        {
            UpdateCar();
        }

        // The method collects the text from the textboxes and sends it to be processed on the SqlCar class
        public void UpdateCar()
        {
            string[] textBoxStrings = { textBox_ID.Text, textBox_Reg.Text, textBox_Make.Text, textBox_Model.Text, textBox_Trim.Text, textBox_Mileage.Text,
                textBox_Colour.Text, textBox_Year.Text, textBox_Price.Text, textBox_URL.Text, textBox_Location.Text,
                dateTimePicker_DateAdded.Text, textBox_MOT.Text };

            SqlCar.UpdateDatabaseEntry(textBoxStrings);
        }

        // Right click to delete the entry in listview
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteCar();
        }

        public void DeleteCar()
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this entry?",
                "Important Question",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                SqlCar.DeleteDatabaseEntry(listView1.SelectedItems[0].SubItems[0].Text);
            }
        }

        // Sorting by selecting an option in the combo box, sort by price (asc/desc)
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = comboBox1.SelectedIndex;
            switch (index)
            {
                case 0:
                    // Try comparison delegate even though it looks weird
                    myCars.Sort((x, y) => x.price.CompareTo(y.price));
                    PopulateListView(myCars);
                    toolStripStatusLabel1.Text = "Sorted by price (ascending)";
                    break;
                case 1:
                    myCars.Sort((y, x) => x.price.CompareTo(y.price));
                    PopulateListView(myCars);
                    toolStripStatusLabel1.Text = "Sorted by price (descending)";
                    break;
                case 2:
                    myCars.Sort((x, y) => x.year.CompareTo(y.year));
                    PopulateListView(myCars);
                    toolStripStatusLabel1.Text = "Sorted by year (asc)";
                    break;
                case 3:
                    myCars.Sort((y, x) => x.year.CompareTo(y.year));
                    PopulateListView(myCars);
                    toolStripStatusLabel1.Text = "Sorted by year (desc)";
                    break;
                default:
                    break;
            }
        }

        // Mileage box limited to digits, Location box limited to letters
        private void textBoxNewMileage_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar)) && !(char.IsControl(e.KeyChar)); // why is it "&&", why is it "!", it seems opposite
        }

        private void textBoxNewLocation_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsLetter(e.KeyChar)) && !(char.IsControl(e.KeyChar));
        }

        // Delete entries from DB by entering a criteria instead of an identifier
        private void buttonDeleteByCriteria_Click(object sender, EventArgs e)
        {
            DeleteByCriteria();
        }

        public void DeleteByCriteria()
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete all cars from the database according to the criteria?",
                "Important Question",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation);

            if (result == DialogResult.Yes)
            {

            }
        }
    }
}
