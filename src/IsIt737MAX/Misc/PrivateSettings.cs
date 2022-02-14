using Microsoft.Extensions.Configuration;
using Sebagomez.TwitterLib.Helpers;

namespace IsIt737MAX.Misc
{
    internal class PrivateSettings
    {
        public string UserToken { get; set; }
        public string UserSecret { get; set; }
        public string AppKey { get; set; }
        public string AppSecret { get; set; }


        public static AuthenticatedUser GetAuthenticatedUser(IConfigurationRoot config)
        {
            PrivateSettings settings = new PrivateSettings();
            config.Bind(settings);

            AuthenticatedUser user = new AuthenticatedUser(settings.UserToken, settings.UserSecret)
            {
                AppSettings = new AppCredentials { AppKey = settings.AppKey, AppSecret = settings.AppSecret }
            };

            return user;
        }

    }
}
