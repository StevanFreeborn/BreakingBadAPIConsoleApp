using System;
using System.Configuration;
using RestSharp;
using Newtonsoft.Json;
using Onspring.API.SDK;
using Onspring.API.SDK.Helpers;
using Onspring.API.SDK.Enums;
using Onspring.API.SDK.Models;

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
            var records = AsyncHelper.RunTask(() => onspringClient.GetRecordAsync(request));
            Console.WriteLine(records);
        }
    }
}
