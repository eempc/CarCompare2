﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Reflection;
using Newtonsoft.Json;

namespace CarCompareDesktop
{
    public class SqlCar
    {
        public int id { get; set; }
        public string registration { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string trim { get; set; }
        public int mileage { get; set; }
        public string colour { get; set; }
        public int year { get; set; }
        public decimal price { get; set; }
        public string url { get; set; }
        public string location { get; set; }
        public DateTime dateAdded { get; set; }
        public int mot { get; set; }

        public SqlCar(int id, string registration, string make, string model, string trim, int mileage, string colour, int year, decimal price, string url, string location, DateTime dateAdded, int mot)
        {
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

        public SqlCar()
        {

        }

        // An instanceof the sql connection, only one can be active at a time?
        public static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Evie\CarCompareContext-d1a204cc-6cb2-4983-b6ca-8e135f56615c.mdf;Integrated Security=True";
        public static SqlConnection connect = new SqlConnection(connectionString);

        public static List<string> GetColumnNames()
        {
            List<string> columnNames = new List<string>();

            connect.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM Car WHERE 1 = 2", connect);
            SqlDataReader reader = command.ExecuteReader();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnNames.Add(reader.GetName(i));
            }

            reader.Close();
            connect.Close();
            return columnNames;
        }

        public static async Task<List<SqlCar>> ReadDatabaseAsync(string commandString)
        {
            List<SqlCar> cars = new List<SqlCar>();

            await connect.OpenAsync();

            SqlCommand command = new SqlCommand(commandString, connect);
            SqlDataReader reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
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
            connect.Close();

            return cars;
        }

        public static void UpdateDatabaseEntry(string[] sqlParameters)
        {
            List<string> columns = GetColumnNames();
            //if (columns.Count != sqlParameters.Length) return;

            // Construct the command string
            string cmdText = "UPDATE Car SET ";
            foreach (string str in columns)
            {
                if (str != "Id")
                {
                    cmdText += str + " = @_" + str + ", ";
                }
            }
            cmdText = cmdText.Trim().TrimEnd(',') + " WHERE Id = @_Id";

            connect.Open();
            SqlCommand updater = connect.CreateCommand();
            updater.CommandText = cmdText;

            // Add the parameters
            for (int i = 0; i < columns.Count; i++)
            {
                updater.Parameters.AddWithValue("@_" + columns[i], sqlParameters[i]);
            }

            updater.ExecuteNonQuery();
            connect.Close();
        }

        // Add one car via manual method (but I am still going to add cars en masse with this method because I want to wrap this project up)
        public static async Task CreateDatabaseEntryAsync(SqlCar newCar)
        {
            List<string> columns = GetColumnNames();
            columns.RemoveAt(0); // Remove id column as it is assigned automatically as opposed to if (str != "Id")
            string cmdStrMiddle = String.Join(",", columns.ToArray());
            cmdStrMiddle.TrimEnd(',');

            List<string> carPropertiesNames = new List<string>();
            foreach (PropertyInfo prop in newCar.GetType().GetProperties())
            {
                if (prop.Name != "id")
                {
                    carPropertiesNames.Add("@_" + prop.Name);
                }
            }
            string cmdStrEnd = String.Join(",", carPropertiesNames.ToArray());
            cmdStrEnd.TrimEnd(',');

            await connect.OpenAsync();

            string commandString = String.Format("INSERT INTO Car (" + cmdStrMiddle + ") VALUES (" + cmdStrEnd + ")");
            SqlCommand command = new SqlCommand(commandString, connect);

            foreach (PropertyInfo prop in newCar.GetType().GetProperties())
            {
                if (prop.Name != "id")
                {
                    command.Parameters.AddWithValue("@_" + prop.Name, prop.GetValue(newCar, null));
                }
            }

            await command.ExecuteNonQueryAsync();
            connect.Close();
        }

        public static void DeleteDatabaseEntry(string id)
        {
            connect.Open();
            SqlCommand deleter = new SqlCommand("DELETE FROM Car WHERE Id = @_Id", connect);
            deleter.Parameters.AddWithValue("@_Id", id);
            deleter.ExecuteNonQuery();
            connect.Close();
        }

        public static List<SqlCar> ReadDatabaseByCondition()
        {
            List<SqlCar> cars = new List<SqlCar>();

            connect.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM Car WHERE Price>500 AND Price<3000", connect);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
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
            connect.Close();
            return cars;
        }

    }
}
