using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sebagomez.TwitterLib.API.Options;
using Sebagomez.TwitterLib.API.Tweets;
using Sebagomez.TwitterLib.Entities;
using Sebagomez.TwitterLib.Helpers;

namespace IsIt737MAX.Misc
{
    public class FlowTweet
	{
		public string TweetText { get; set; }
		public string TweetId { get; set; }
		public string CreatedAt { get; set; }
		public string CreatedAtIso { get; set; }
		public int RetweetCount { get; set; }
		public string TweetedBy { get; set; }
		public object[] MediaUrls { get; set; }
		public string TweetLanguageCode { get; set; }
		public string TweetInReplyToUserId { get; set; }
		public bool Favorited { get; set; }
		public Usermention[] UserMentions { get; set; }
		public object OriginalTweet { get; set; }
		public Userdetails UserDetails { get; set; }

		public static FlowTweet GetFromAzureFunction(Stream stream)
        {
			return Util.Deserialize<FlowTweet>(stream);
		}

		public static async Task<FlowTweet> GetIFTTT(Stream stream, IConfigurationRoot config)
		{
			string twitUrl;
			using (StreamReader reader = new StreamReader(stream))
				twitUrl = reader.ReadToEnd();

			Regex regex = new Regex(@"https?:\/\/.*\/([0-9]+)\/?");
			Match match = regex.Match(twitUrl);
			if (match.Success)
            {
				Group g = match.Groups[1];
				string twitId = g.Value;

				PrivateSettings settings = new PrivateSettings();
				config.Bind(settings);

				AuthenticatedUser user = new AuthenticatedUser(settings.UserToken, settings.UserSecret);
				user.AppSettings = new AppCredentials { AppKey = settings.AppKey, AppSecret = settings.AppSecret };

				Status s = await Timeline.GetStatus(new StatusOptions { User = user, Id = twitId });

				return FlowTweet.FromStatus(s);
			}


			throw new System.Exception($"Invalid url: {twitUrl}");
		}

        private static FlowTweet FromStatus(Status s)
        {
			FlowTweet twit = new FlowTweet
			{
				TweetId = s.id.ToString(),
				TweetText = s.text,
				UserDetails = new Userdetails { Id = s.user.id, UserName = s.user.screen_name }
			};

			return twit;
        }
    }

	public class Userdetails
	{
		public string FullName { get; set; }
		public string Location { get; set; }
		public long Id { get; set; }
		public string UserName { get; set; }
		public int FollowersCount { get; set; }
		public string Description { get; set; }
		public int StatusesCount { get; set; }
		public int FriendsCount { get; set; }
		public int FavouritesCount { get; set; }
		public string ProfileImageUrl { get; set; }
	}

	public class Usermention
	{
		public long Id { get; set; }
		public string FullName { get; set; }
		public string UserName { get; set; }
	}

}
