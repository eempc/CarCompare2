using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

// It is possible to access Form1's controls from here but it is not a good idea. E.g. TextBox t = Application.OpenForms["Form1"].Controls["textBox1"] as TextBox;
// It is better to use return values from this class to Form1

namespace CarCompareDesktop {
    public class SqlCar {
        // Car and DB attributes go here
        //public int CarId { get; set; }
        //public string CarRegNo { get; set; }
        //public string CarMake { get; set; }
        //public string CarModel { get; set; }
        //public string CarTrim { get; set; }
        //public int CarMileage { get; set; }
        //public string CarColour { get; set; }
        //public decimal CarPrice { get; set; }
        //public string CarUrl { get; set; }
        //public string CarLocation { get; set; }
        //public DateTime CarDateAdded { get; set; }
        //public int CarMotExpiry { get; set; }

        public int id, mileage, mot, year;
        public string registration, make, model, trim, colour, url, location;
        public decimal price;
        public DateTime dateAdded;

        public SqlCar(int id, string registration, string make, string model, string trim, int mileage, string colour, int year, decimal price, string url, string location, DateTime dateAdded, int mot) {
            this.id = id;
            this.registration = registration;
            this.make = make;
            this.model = model;
            this.trim = trim;               
            this.mileage = mileage;
            this.colour = colour;
            this.year = year;
            this.price = price;
            this.url = url;
            this.location = location;
            this.dateAdded = dateAdded;
            this.mot = mot;
        }

        public static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Evie\CarCompareContext-d1a204cc-6cb2-4983-b6ca-8e135f56615c.mdf;Integrated Security=True";

        public static List<string> GetColumnNames() {
            SqlConnection connection = new SqlConnection(connectionString);

            List<string> columnNames = new List<string>();

            connection.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM Car WHERE 1 = 2", connection);
            SqlDataReader reader = command.ExecuteReader();

            for (int i = 0; i < reader.FieldCount; i++) {
                columnNames.Add(reader.GetName(i));
            }
            
            reader.Close();
            connection.Close();
            return columnNames;
        }

        public static List<SqlCar> AccessSqlReader(string commandString) {
            SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Evie\CarCompareContext-d1a204cc-6cb2-4983-b6ca-8e135f56615c.mdf;Integrated Security=True");

            List<SqlCar> cars = new List<SqlCar>();

            connection.Open();
            SqlCommand command = new SqlCommand(commandString, connection);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read()) {
                cars.Add(
                    new SqlCar(
                        reader.GetInt32(0), 
                        reader.GetString(1), 
                        reader.GetString(2), 
                        reader.GetString(3), 
                        reader.GetString(4), 
                        reader.GetInt32(5),
                        reader.GetString(6),
                        reader.GetInt32(7),
                        reader.GetDecimal(8),
                        reader.GetString(9),
                        reader.GetString(10),
                        reader.GetDateTime(11),
                        reader.GetInt32(12)
                    )
                );
            }
            reader.Close();
            connection.Close();

            return cars;
        }

    }
}
