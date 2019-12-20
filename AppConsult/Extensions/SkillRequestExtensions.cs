using System;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;

namespace AppConsult.Extensions
{
    public static class SkillRequestExtensions
    {
        public static bool IntentDoesNotMatch(
            this SkillRequest skillRequest,
            string intentName)
        {
            var intentRequest = skillRequest.Request as IntentRequest;

            return intentRequest?.Intent.Name != intentName;
        }

        public static bool IsRequestType<TIntentType>(
            Type requestType) where TIntentType : class
        {
            return requestType == typeof(TIntentType);
        }

        public static bool IsRequestType<TIntentType>(
            this SkillRequest skillRequest) where TIntentType : class
        {
            var requestType = skillRequest.GetRequestType();
            return requestType != typeof(TIntentType);
        }

        public static bool IsNotRequestType<TIntentType>(
            this SkillRequest skillRequest) where TIntentType : class
        {
            var requestType = skillRequest.GetRequestType();
            return requestType != typeof(TIntentType);
        }

        public static bool IntentDoesNotMatch(
            string intentName,
            SkillRequest skillRequest)
        {
            var intentRequest = skillRequest.Request as IntentRequest;

            return intentRequest?.Intent.Name != intentName;
        }
    }
}