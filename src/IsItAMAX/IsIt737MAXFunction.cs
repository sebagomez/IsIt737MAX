using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IsIt737MAX.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sebagomez.ShelltwitLib.API.OAuth;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.API.Tweets;
using Sebagomez.ShelltwitLib.Helpers;

namespace IsIt737MAX
{
	public static class IsIt737MAXFunction
	{
		static IConfigurationRoot s_config = null;
		const string CONF_FILE = "secret.settings.json";

		[FunctionName("IsIt737MAX")]
		public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log, ExecutionContext context)
		{
			try
			{
				log.LogInformation("C# HTTP trigger function processed a request.");

				if (!File.Exists(Path.Combine(context.FunctionAppDirectory, CONF_FILE)))
					throw new FileNotFoundException("Config file not found");

				s_config = new ConfigurationBuilder()
					.SetBasePath(context.FunctionAppDirectory)
					.AddJsonFile(CONF_FILE, optional: true, reloadOnChange: true)
					.AddEnvironmentVariables()
					.Build();

				FlowTweet tweet = Util.Deserialize<FlowTweet>(req.Body);

				log.LogInformation($"{tweet.TweetId}({tweet.UserDetails.UserName}):{tweet.TweetText}");
				if (tweet.UserDetails.Id == 1108721422418014211) // that was me
					return new OkObjectResult($"Nothing to do, it was my own twit:{tweet.TweetText}");

				string flightNum = tweet.TweetText.Replace("@IsIt737MAX", "", StringComparison.CurrentCultureIgnoreCase).Trim();
				if (flightNum.Length > 10)
					return new OkObjectResult($"Nothing to do, does not look like a valid flight number:{flightNum}, original text:{tweet.TweetText}");

				var (airline, number) = FlightNumberParser.Parse(flightNum);
				string errorMsg = $"{flightNum} does not look like a valid flight number. Please send me the 2 or 3 characters airline code followed by the flight number";

				if (string.IsNullOrEmpty(airline) || string.IsNullOrEmpty(number))
				{
					await TwitStatus(tweet.TweetId, errorMsg);
					return new BadRequestObjectResult($"{errorMsg}");
				}

				log.LogInformation($"Airline:{airline} Number:{number}");

				var (aircraft, ident) = await FlightAwareHelper.GetAircraftType(airline, number);

				log.LogInformation($"Aircraft:{aircraft} Ident:{ident}");

				string message = GetTwitMessage(aircraft, ident);
				if (string.IsNullOrEmpty(message))
				{
					await TwitStatus(tweet.TweetId, errorMsg);
					return new BadRequestObjectResult($"{errorMsg}");
				}

				string status = $"@{tweet.UserDetails.UserName} {message}";

				log.LogInformation($"Twit:{status}");

				string response = await TwitStatus(tweet.TweetId, status);

				log.LogInformation($"{response}");

				return response == "OK" ? (IActionResult)new OkObjectResult($"{status}") : new BadRequestObjectResult(response);
			}
			catch (Exception ex)
			{
				log.LogError(ex, "Something bad happened!");
				return new BadRequestObjectResult(ex);
			}
		}

		static async Task<string> TwitStatus(string replyId, string status)
		{
			PrivateSettings settings = new PrivateSettings();
			s_config.Bind(settings);

			OAuthAuthenticator.Initilize(settings.AppKey, settings.AppSecret);
			AuthenticatedUser user = new AuthenticatedUser(settings.UserToken, settings.UserSecret);

			return await Update.UpdateStatus(new UpdateOptions { ReplyId = replyId, Status = status, User = user });
		}

		static string GetTwitMessage(string aircraft, string flightDescription)
		{
			if (string.IsNullOrEmpty(aircraft) && string.IsNullOrEmpty(flightDescription))
				return null;

			if (string.IsNullOrEmpty(aircraft))
				return $"Sorry, I couldn't find the aircraft type for {flightDescription}";

			aircraft = Regex.Replace(aircraft, @"\(.*\)", "").Trim();

			string message = "";
			string singleAirgractType = $"{(aircraft.StartsWith("A") ? "an" : "a")} {aircraft}";
			if (aircraft.Contains("737") && aircraft.ToUpper().Contains("MAX"))
				message = $"You are fliying a MAX!.";
			else if (aircraft.Contains("737"))
				message = $"You are flying a 737, but it's not a MAX.";
			else if (aircraft.ToUpper().Contains("BOEING"))
				message = $"You are flying a Boeing, but it's not a 737.";
			else
				message = $"Chill. You're not even flying a Boeing.";

			if (!string.IsNullOrEmpty(flightDescription))
				message += $" {flightDescription} is usually {singleAirgractType}.";
			else
				message += $" It's {singleAirgractType}.";

			return message;
		}
	}
}
