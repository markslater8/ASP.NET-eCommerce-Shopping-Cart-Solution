﻿using System.Text.RegularExpressions;
using Amazon.Pay.API.Types;
using Newtonsoft.Json;

namespace Smartstore.AmazonPay
{
    internal static class ApiRequestBodyExtensions
    {
        /// <summary>
        /// This is a temporary workaround for the following SDK issue:
        /// https://github.com/amzn/amazon-pay-api-sdk-dotnet/issues/11
        /// </summary>
        public static string AsJson(this ApiRequestBody body)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None
            };

            var jsonString = JsonConvert.SerializeObject(body, serializerSettings);

            // remove empty objects from the JSON string
            var regex = new Regex(",?\"[a-z]([a-z]|[A-Z])+\":{}");
            jsonString = regex.Replace(jsonString, string.Empty);

            // remove potential clutter
            var regex2 = new Regex("{,\"");
            jsonString = regex2.Replace(jsonString, "{\"");

            return jsonString;
        }
    }
}