using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sebagomez.TwitterLib.API.OAuth;
using Sebagomez.TwitterLib.API.Options;
using Sebagomez.TwitterLib.API.Tweets;
using Sebagomez.TwitterLib.Entities;
using Sebagomez.TwitterLib.Helpers;

namespace IsIt737MAX.Misc
{
    internal class TwitterHelper
    {
        const string OK = "OK";
        const string CRC_TOKEN = "crc_token";
        const string RESPONSE_TOKEN = "response_token";
        const long MY_ID = 1108721422418014211;

        const int MAX_LENGTH = 8;

        const string EMPTY_CRC = "Empty crc_token";
        const string NOT_FOR_ME = "Nothing to do, it is not for me.";
        const string NOT_A_MENTION = "Nothing to do, it is not a mention.";
        const string IT_WAS_ME = "Nothing to do, it was my own twit.";

        IConfigurationRoot m_config = null;
        ILogger m_log = null;

        public TwitterHelper(IConfigurationRoot config, ILogger log)
        {
            m_config = config;
            m_log = log;
        }

        public IActionResult ValidateTwitterCRC(HttpRequest req)
        {
            m_log.LogInformation($"QueryString:{req.QueryString}");
            string crc_token = req.Query[CRC_TOKEN];
            if (string.IsNullOrWhiteSpace(crc_token))
                throw new InvalidOperationException(EMPTY_CRC);

            m_log.LogInformation($"{CRC_TOKEN} is {crc_token}");

            PrivateSettings settings = new PrivateSettings();
            m_config.Bind(settings);

            string response_token = OAuthAuthenticator.GetCRCResponseToken(settings.AppSecret, crc_token);

            return (IActionResult)new OkObjectResult($"{{\"{RESPONSE_TOKEN}\": \"sha256={response_token}\"}}");
        }

        public async Task<IActionResult> SendOutTwit(HttpRequest req)
        {
            TwitterEvent twitEvent;
            try
            {
                //twit = twitEvent.tweet_create_events[0];
                //tweet = await FlowTweet.GetIFTTT(req.Body, m_config);
                //tweet = FlowTweet.GetFromAzureFunction(req.Body);
                //TwitterEvent twitEvent = Util.Deserialize<TwitterEvent>(req.Body);
                twitEvent = TwitterEvent.FromBody(req.Body, m_log);
                if (twitEvent.for_user_id != MY_ID.ToString())
                {
                    m_log.LogInformation(NOT_FOR_ME);
                    return new OkObjectResult(NOT_FOR_ME);
                }

                if (twitEvent.tweet_create_events == null)
                {
                    m_log.LogInformation(NOT_A_MENTION);
                    return new OkObjectResult(NOT_A_MENTION);
                }
            }
            catch (Exception ex)
            {
                m_log.LogError(ex, "Something bad happened!");
                return new BadRequestObjectResult(ex);
            }

            bool ok = true;
            foreach (Status twit in twitEvent.tweet_create_events)
                ok &= await ProcessMention(twit);
            
            return (IActionResult)new OkObjectResult(OK);
        }

        private async Task<bool> ProcessMention(Status mention)
        {
            m_log.LogInformation($"{mention.id}({mention.user.name}):{mention.text}");
            if (mention.user.id == MY_ID) // that was me
            {
                m_log.LogInformation(IT_WAS_ME);
                return false;
            }

            string flightNum = mention.text.Replace("@IsIt737MAX", string.Empty, StringComparison.CurrentCultureIgnoreCase);
            flightNum = flightNum.Replace(" ", string.Empty, StringComparison.CurrentCultureIgnoreCase);
            if (flightNum.Length > MAX_LENGTH)
            {
                m_log.LogInformation($"Nothing to do, does not look like a valid flight number:{flightNum}, original text:{mention.text}");
                return false;
            }

            var (airline, number) = FlightNumberParser.Parse(flightNum);
            string errorMsg = $"@{mention.user.screen_name} {flightNum} does not look like a valid flight number. Please send me the 2 or 3 characters airline code followed by the flight number";

            if (string.IsNullOrEmpty(airline) || string.IsNullOrEmpty(number))
            {
                m_log.LogInformation(errorMsg);
                await TwitStatus(mention.id.ToString(), errorMsg);
                return true;
            }

            m_log.LogInformation($"Airline:{airline} Number:{number}");

            var (aircraft, ident) = await FlightDataHelper.GetAircraftType(airline, number);

            m_log.LogInformation($"Aircraft:{aircraft} Ident:{ident}");

            string message = GetTwitMessage(aircraft, ident);
            if (string.IsNullOrEmpty(message))
            {
                m_log.LogInformation(errorMsg);
                await TwitStatus(mention.id.ToString(), errorMsg);
                return true;
            }

            string status = $"@{mention.user.screen_name} {message}";

            m_log.LogInformation($"Twit:{status}");

            string response = await TwitStatus(mention.id.ToString(), status);

            m_log.LogInformation($"{response}");

            return response == OK;
        }

        private async Task<string> TwitStatus(string replyId, string status)
        {
            PrivateSettings settings = new PrivateSettings();
            m_config.Bind(settings);

            AuthenticatedUser user = new AuthenticatedUser(settings.UserToken, settings.UserSecret)
            {
                AppSettings = new AppCredentials { AppKey = settings.AppKey, AppSecret = settings.AppSecret }
            };

            return await Update.UpdateStatus(new UpdateOptions { ReplyId = replyId, Status = status, User = user });
        }

        private string GetTwitMessage(string aircraft, string flightDescription)
        {
            if (string.IsNullOrEmpty(aircraft) && string.IsNullOrEmpty(flightDescription))
                return null;

            if (string.IsNullOrEmpty(aircraft))
                return $"Sorry, I couldn't find the aircraft type for {flightDescription}";

            aircraft = Regex.Replace(aircraft, @"\(.*\)", "").Trim();
            aircraft = Regex.Replace(aircraft, @"\/.*", "").Trim();
            Match m = Regex.Match(aircraft, @"([\w\s\d-]*)");
            aircraft = m.Value.Trim();
            string singleAirgractType = $"{(aircraft.StartsWith("A") ? "an" : "a")} {aircraft}";

            string message;
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
