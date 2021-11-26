using System;
using System.Configuration;
using Onspring.API.SDK;
using Onspring.API.SDK.Helpers;
using Onspring.API.SDK.Enums;
using Onspring.API.SDK.Models;
using static Utility;

namespace consoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            // create onspring api client.
            var onspringBaseUrl = ConfigurationManager.AppSettings["baseUrl"]; ;
            var apiKey = ConfigurationManager.AppSettings["apiKey"];
            var onspringClient = new OnspringClient(onspringBaseUrl, apiKey);
            
            // verify connectivity to onspring api.
            bool onspringCanConnect = AsyncHelper.RunTask(() => onspringClient.CanConnectAsync());
            
            // create breaking bad api client.
            const string breakingBadBaseUrl = "https://www.breakingbadapi.com/api/";
            var breakingBadApi = new BreakingBadHelper(breakingBadBaseUrl);

            // verify connectivity to breaking bad api.
            bool breakingBadCanConnect = breakingBadApi.CanConnect();

            if(onspringCanConnect && breakingBadCanConnect)
            {
                Console.WriteLine("Connected successfully.");
            }
            else if(!onspringCanConnect && breakingBadCanConnect)
            {
                Console.WriteLine("Could not connect to the Onspring API.");
            }
            else if(onspringCanConnect && !breakingBadCanConnect)
            {
                Console.WriteLine("Could not connect to the Breaking Bad API.");
            }
            else
            {
                Console.WriteLine("Unable to connect to either the Onspring API or the Breaking Bad API.");
            }

            var request = new GetRecordRequest(357,1);
            var getResponse = AsyncHelper.RunTask(() => onspringClient.GetRecordAsync(request));
            var record = getResponse.Value;

            foreach (var recordFieldValue in record.FieldData)
            {
                Console.WriteLine($"FieldId: {recordFieldValue.FieldId}, Type: {recordFieldValue.Type}, Value: {GetResultValueString(recordFieldValue)}");
            }
        }
    }
}
