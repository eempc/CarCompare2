using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

// Retrieve web page then extract the JSON then deserialize the JSON
// Do I need the Chrome Driver?

namespace CarCompareDesktop {
    class WebScraper {

        // Simple web client call (can include local files)
        public static string GetHtmlViaWebClient(string url) {
            WebClient client = new WebClient();
            return client.DownloadString(url);
        }

        // URL must be HTTP
        public static async Task<string> GetHtmlViaHttpClientAsync(string url) {
            using (HttpClient client = new HttpClient()) {
                Task<string> getHtmlTask = client.GetStringAsync(url);
                // Independent work can go in here
                string html = await getHtmlTask;
                return html;
                // string html = await client.GetStringAsync(url) // Shorter method if you don't need to do the two step version of setting up a task then executing it
            }
        }

        public static string ExtractViaRegex(string source, string start, string end)
        {
            Regex rx = new Regex(start + "(.*?)" + end);
            var match = rx.Match(source);
            return match.Groups[1].ToString();
        }

        //static Func<string, string, string, string> Extract = (source, start, end) => ExtractViaRegex(source, start, end);

        public static string ExtractGumtreeJSON(string html)
        {
            return ExtractViaRegex(html, "var dataLayer = ", "];");            
        }


        public static SqlCar NewGumtreeCar(string html)
        {
            string gumtreeJsonDataLayer = ExtractGumtreeJSON(html);
            gumtreeJsonDataLayer = gumtreeJsonDataLayer.TrimStart('[');
            dynamic obj = JsonConvert.DeserializeObject(gumtreeJsonDataLayer);

            SqlCar gumtreeCar = new SqlCar();
            gumtreeCar.id = -1;
            gumtreeCar.registration = obj.a.attr.vrn;
            gumtreeCar.make = obj.a.attr.vehicle_make;
            gumtreeCar.model = obj.a.attr.vehicle_model;
            gumtreeCar.trim = "N/A";
            gumtreeCar.mileage = obj.a.attr.vehicle_mileage;
            gumtreeCar.colour = obj.a.attr.vehicle_colour;
            gumtreeCar.year = obj.a.attr.vehicle_registration_year;
            gumtreeCar.price = Convert.ToDecimal(obj.a.attr.price) / 100;
            gumtreeCar.url = ExtractViaRegex(html, "https://", " -->");
            gumtreeCar.location = obj.l.pcid;
            gumtreeCar.dateAdded = DateTime.Today;
            gumtreeCar.mot = 0;

            return gumtreeCar;
        }


    }
}
