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
            
            const string onspringBaseUrl = "https://api.alpha.onspring.ist";
            const string apiKey = "61546a78a65cf5787573c39a/2b02fc67-e3c4-4292-8438-201e1ecec61d";
            var onspringClient = new OnspringClient(onspringBaseUrl, apiKey);

            bool canConnect = AsyncHelper.RunTask(() => onspringClient.CanConnectAsync());

            if(canConnect)
            {
                Console.WriteLine("Connected successfully.");
            }
            else
            {
                Console.WriteLine("Unable to connect.");
            }

            var apps = AsyncHelper.RunTask(() => onspringClient.GetAppsAsync());

            Console.WriteLine(apps.Value);
            

            /*
            const string breakingBadBaseUrl = "https://www.breakingbadapi.com/api/";
            var breakingBadApi = new BreakingBadHelper(breakingBadBaseUrl);;
            var print = breakingBadApi.GetACharacter();

            Console.WriteLine(print);
            */
        }
    }
}
