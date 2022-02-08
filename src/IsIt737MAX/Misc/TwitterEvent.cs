using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using Sebagomez.TwitterLib.Entities;

namespace IsIt737MAX.Misc
{
    public class TwitterEvent
    {
        public string for_user_id { get; set; }
        public bool user_has_blocked { get; set; }
        public List<Status> tweet_create_events { get; set; }

        public static TwitterEvent FromBody(Stream stream,ILogger log)
        {
            string strData;
            using (StreamReader reader = new StreamReader(stream))
                strData = reader.ReadToEnd();

            string prettyJson = "";
            try { 
                prettyJson = Utilities.PrettyJson(strData);
            }
            catch (Exception)
            {
                prettyJson = strData;
            }

            System.Diagnostics.Debug.Write(prettyJson);
            log.LogInformation(prettyJson);

            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(strData)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TwitterEvent));
                return (TwitterEvent)serializer.ReadObject(ms);
            }
        }


    }
}
