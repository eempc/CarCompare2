using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;


// Retrieve web page then extract the JSON then deserialize the JSON
// Do I need the Chrome Driver?

namespace CarCompareDesktop {
    class WebScraper {

        //Non-async simple web client call (can include local files)
        public static string GetHtmlViaWebClient(string url) {
            WebClient client = new WebClient();
            return client.DownloadString(url);
        }

        // 'Async' should be in the method name - url must be an HTTP request
        public static async Task<string> GetHtmlViaHttpClientAsync(string url) {
            using (HttpClient client = new HttpClient()) {
                Task<string> getHtmlTask = client.GetStringAsync(url);
                // Independent work can go in here
                string html = await getHtmlTask;
                return html;
                // string html = await client.GetStringAsync(url) // Shorter method if you don't need to do the two step version of setting up a task then executing it
            }
        }
    }
}
