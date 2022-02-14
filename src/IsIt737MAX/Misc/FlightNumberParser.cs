using System.Text.RegularExpressions;

namespace IsIt737MAX.Misc
{
    internal class FlightNumberParser
    {
        const int MAX_LENGTH = 8;
        const string IATA_ICAO_REGEX = "^([A-Za-z]{2}|[A-Za-z]{3}|[A-Za-z][0-9]|[0-9][A-Za-z])([0-9]+[A-Za-z]?)$";

        public static (string airline, string number) Parse(string flightNumber)
        {
            if (flightNumber.Length > MAX_LENGTH)
                return (string.Empty, string.Empty);

            string airline = "", number = "";
            flightNumber = flightNumber.Trim().Replace(" ", "");

            Match match = Regex.Match(flightNumber, IATA_ICAO_REGEX);

            if (match.Success)
            {
                airline = match.Groups[1].Value;
                number = match.Groups[2].Value;
            }

            return (airline, number);
        }
    }
}
