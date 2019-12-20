using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.SyndicationFeed.Rss;
using Newtonsoft.Json;

namespace AppConsult
{
    public static class Alexa
    {
        [FunctionName("Alexa")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var json = await req.ReadAsStringAsync();
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(json);

            var requestType = skillRequest.GetRequestType();

            SkillResponse response = null;

            if (requestType == typeof(LaunchRequest))
            {
                response = ResponseBuilder.Tell("Welcome to AppConsult!");
                response.Response.ShouldEndSession = false;
            }

            else if (requestType == typeof(IntentRequest))
            {
                var intentRequest = skillRequest.Request as IntentRequest;

                if (intentRequest?.Intent.Name != "LastPosts")
                {
                    return new OkObjectResult((SkillResponse) null);
                }

                const string rss = "https://blogs.msdn.microsoft.com/appconsult/feed/";

                var news = await ParseFeed(rss);

                var output = $"The title of the last article is {news.FirstOrDefault()}";

                response = ResponseBuilder.Tell(output);
            }

            return new OkObjectResult(response);
        }

        private static async Task<List<string>> ParseFeed(string url)
        {
            var news = new List<string>();

            using (var xmlReader = XmlReader.Create(url, new XmlReaderSettings { Async = true }))
            {
                var feedReader = new RssFeedReader(xmlReader);
                while (await feedReader.Read())
                {
                    if (feedReader.ElementType != Microsoft.SyndicationFeed.SyndicationElementType.Item)
                    {
                        continue;
                    }

                    var item = await feedReader.ReadItem();
                    news.Add(item.Title);
                }
            }

            return news;
        }
    }
}