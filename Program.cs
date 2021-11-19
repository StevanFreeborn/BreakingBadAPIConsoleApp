using System;
using RestSharp;
using Newtonsoft.Json;
using Onspring.API.SDK;
using Onspring.API.SDK.Helpers;

namespace consoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            // create onspring api client.
            const string onspringBaseUrl = "https://api.alpha.onspring.ist";
            const string apiKey = "61546a78a65cf5787573c39a/2b02fc67-e3c4-4292-8438-201e1ecec61d";
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
        }
    }
}
