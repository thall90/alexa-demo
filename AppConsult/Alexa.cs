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
using AppConsult.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using Newtonsoft.Json;

namespace AppConsult
{
    public static class Alexa
    {
        private const string rssFeed = "https://techcommunity.microsoft.com/gxcuf89792/plugins/custom/microsoft/o365/custom-blog-rss?board=WindowsDevAppConsult&label=&messages=&size=10";

        [FunctionName("Alexa")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request,
            ILogger log)
        {
            var skillRequest = await request.DeserializeSkillRequest();

            if (skillRequest.IsRequestType<LaunchRequest>())
            {
                return new OkObjectResult(CreateLaunchResponse("Welcome to AppConsult!"));
            }

            if (skillRequest.IsNotRequestType<IntentRequest>())
            {
                return new OkObjectResult(null);
            }

            if (skillRequest.IntentDoesNotMatch("LastPosts"))
            {
                return new OkObjectResult(null);
            }

            var response = await CreateLastPostIntentResponse();

            return new OkObjectResult(response);
        }

        private static async Task<SkillResponse> CreateLastPostIntentResponse()
        {
            var news = await ParseFeed(rssFeed);

            var output = ConstructFeedOutput(news);

            var response = ResponseBuilder.Tell(output);
            return response;
        }

        private static SkillResponse CreateLaunchResponse(
            string responseText)
        {
            var response = ResponseBuilder.Tell(responseText);
            response.Response.ShouldEndSession = false;
            return response;
        }

        private static async Task<List<string>> ParseFeed(string url)
        {
            var news = new List<string>();

            using (var xmlReader = XmlReader.Create(url, new XmlReaderSettings { Async = true }))
            {
                var feedReader = new RssFeedReader(xmlReader);
                while (await feedReader.Read())
                {
                    if (IsNotFeedItem(feedReader))
                    {
                        continue;
                    }

                    var item = await feedReader.ReadItem();
                    news.Add(item.Title);
                }
            }

            return news;
        }

        private static bool IsNotFeedItem(
            ISyndicationFeedReader feedReader)
        {
            return feedReader.ElementType != SyndicationElementType.Item;
        }

        private static string ConstructFeedOutput(
            IEnumerable<string> news)
        {
            return $"The title of the last article is \"{news.FirstOrDefault()}\"";
        }
    }
}