using System;
using System.Configuration;
using System.Collections.Generic;
using Onspring.API.SDK;
using Onspring.API.SDK.Helpers;
using Onspring.API.SDK.Enums;
using Onspring.API.SDK.Models;
using Newtonsoft.Json;
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
            var onspringAPI = new OnspringHelper(onspringBaseUrl, apiKey);
            
            // verify connectivity to onspring api.
            bool onspringCanConnect = AsyncHelper.RunTask(() => onspringAPI._client.CanConnectAsync());
            
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

            Console.WriteLine("\n");
            Console.WriteLine("Retrieving random character from thebreakingbadapi.com.");
            var randomCharacter = breakingBadApi.GetARandomCharacter();
            Console.WriteLine("\n");
            Console.WriteLine(JsonConvert.SerializeObject(randomCharacter));

            var character = onspringAPI.GetCharacterById("1");
            Console.WriteLine(JsonConvert.SerializeObject(character));

            var occupation = onspringAPI.GetOccupationByRecordId("Test");
            Console.WriteLine(JsonConvert.SerializeObject(occupation));
        }
    }
}
