using System;

namespace IsIt737MAX.Misc
{
	internal class FlightNumberParser
	{
		public static (string airline, string number) Parse(string flightNumber)
		{
			string airline = "", number = "";
			flightNumber = flightNumber.Trim().Replace(" ", "");

			for (int i = 0; i < flightNumber.Length; i++)
			{
				char c = flightNumber[i];
				if (c > 64 && string.IsNullOrEmpty(number))
					airline += c;
				else
				{
					if (i <= 1)
						airline += c;
					else
						number += c;
				}
			}

			return (airline, number);
		}
	}
}
