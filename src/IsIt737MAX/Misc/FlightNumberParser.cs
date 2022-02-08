using System.Text.RegularExpressions;

namespace IsIt737MAX.Misc
{
    internal class FlightNumberParser
	{

		const string IATA_ICAO_REGEX = "^([A-Za-z]{2}|[A-Za-z]{3}|[A-Za-z][0-9]|[0-9][A-Za-z])([0-9]+[A-Za-z]?)$";

		public static (string airline, string number) Parse(string flightNumber)
		{
			string airline = "", number = "";
			flightNumber = flightNumber.Trim().Replace(" ", "");

			Match match = Regex.Match(flightNumber, IATA_ICAO_REGEX);

			if (match.Success)
            {
				airline = match.Groups[1].Value;
				number = match.Groups[2].Value;
			}

			//for (int i = 0; i < flightNumber.Length; i++)
			//{
			//	char c = flightNumber[i];
			//	if (c < 48 || c > 122 || (c > 57 && c < 65)) // Invalid character found!
			//		return (string.Empty, string.Empty);

			//	if (c > 64 && string.IsNullOrEmpty(number))
			//		airline += c;
			//	else
			//	{
			//		if (i <= 1)
			//			airline += c;
			//		else
			//			number += c;
			//	}
			//}

			return (airline, number);
		}
	}
}
