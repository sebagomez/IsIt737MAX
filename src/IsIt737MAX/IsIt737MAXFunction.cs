using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using IsIt737MAX.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IsIt737MAX
{
    public static class IsIt737MAXFunction
    {
        static IConfigurationRoot s_config = null;
        const string CONF_FILE = "secret.settings.json";

        [FunctionName("IsIt737MAX")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log, ExecutionContext context)
        {
            try
            {
                log.LogInformation($"C# HTTP trigger function processed a {req.Method} request at {DateTime.Now.ToString("ddd, MMMM d, yyyy HH:mm:ss")}");

                string configFilePath = Path.Combine(context.FunctionAppDirectory, CONF_FILE);
                if (!File.Exists(configFilePath))
                    throw new FileNotFoundException($"Config file not found: {configFilePath}", configFilePath);

                s_config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile(CONF_FILE, optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                TwitterHelper twitter = new TwitterHelper(s_config, log);

                if (req.Method == "POST")
                {
                    return await twitter.SendOutTwit(req);
                }
                else if (req.Method == "GET")
                {
                    return await Task.Run(() => twitter.ValidateTwitterCRC(req));
                }

                throw new Exception($"Invalid http method: {req.Method}");

            }
            catch (Exception ex)
            {
                return new ExceptionResult(ex, true);
            }
        }
    }
}
