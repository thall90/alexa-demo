using System.Threading.Tasks;
using Alexa.NET.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace AppConsult.Extensions
{
    public static class HttpRequestExtensions
    {
        public static async Task<SkillRequest> DeserializeSkillRequest(
            this HttpRequest req)
        {
            var json = await req.ReadAsStringAsync();
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(json);
            return skillRequest;
        }
    }
}