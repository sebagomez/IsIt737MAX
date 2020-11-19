using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace IsIt737MAX.Misc
{
	class FlightDataHelper
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
			var response = await request.GetResponseAsync();
			string responseString;
			using (var stream = response.GetResponseStream())
			{
				using (var reader = new StreamReader(stream))
				{
					responseString = reader.ReadToEnd();
				}
			}

			//File.WriteAllText("/Users/seba/dev/seba/IsIt737MAX/flight.html", responseString);

			string aircraft = GetValue(responseString, AIRCRAFT_TYPE);
			string ident = GetValue(responseString, FRIENDLY_IDENT);

			return (aircraft, $"{ident} {number}");
		}

		static string GetValue(string responseString, string key)
		{
			int i = responseString.IndexOf(key);
			if (i < 0)
				return null;

			i += key.Length;
			int j = responseString.IndexOf("\"", i + 1);
			return responseString.Substring(i, j - i);
		}
	}
}
