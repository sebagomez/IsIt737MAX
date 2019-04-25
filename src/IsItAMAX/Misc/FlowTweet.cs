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
