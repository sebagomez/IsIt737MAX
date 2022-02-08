using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IsIt737MAX.Misc
{
    public class FlightDataHelper
    {

        const string TEMPLATE_URL = "https://www.radarbox.com/data/flights/{0}{1}";
        //const string TEMPLATE_URL = "http://flightaware.com/live/flight/{0}{1}";
        const string AIRCRAFT_TYPE = "acd\":\"";
        //const string AIRCRAFT_TYPE = "aircraftTypeFriendly\":\"";
        const string FRIENDLY_IDENT = "alna\":\"";
        //const string FRIENDLY_IDENT = "friendlyIdent\":\"";

        public static async Task<(string aircraft, string ident)> GetAircraftType(string airline, string number)
        {
            string url = string.Format(TEMPLATE_URL, airline, number);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:96.0) Gecko/20100101 Firefox/96.0");
            var response = await request.GetResponseAsync();
            string responseString;
            using (var stream = response.GetResponseStream())
            {
                using var reader = new StreamReader(stream);
                responseString = reader.ReadToEnd();
            }

            RadarBox radarBox = JsonConvert.DeserializeObject<RadarBox>(GetFlightJson(responseString));

            string aircraft = "";
            string ident = "";
            for (int q = 0; q < radarBox.list.list.Count; q++)
            {
                if (aircraft.Length < radarBox.list.list[q].acd?.Length)
                    aircraft = radarBox.list.list[q].acd;

                if (ident.Length < radarBox.list.list[q].csalna?.Length)
                    ident = radarBox.list.list[q].csalna;
            }


            if (aircraft.Contains("/"))
                aircraft = aircraft.Substring(0, aircraft.IndexOf("/"));

            return (aircraft, $"{ident} {number}");
        }

        private static string GetFlightJson(string responseString)
        {
            string start = "<script>window.init(";
            string end = ")</script>";
            int i = responseString.IndexOf(start);
            responseString = responseString.Substring(i + start.Length);
            int j = responseString.IndexOf(end);
            responseString = responseString.Substring(0, j);
            return responseString;
        }
    }
}
