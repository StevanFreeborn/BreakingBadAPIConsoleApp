using System;
using System.Configuration;
using Onspring.API.SDK.Helpers;

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
            Console.WriteLine("Connecting to Onspring API...");

            // verify connectivity to onspring api.
            bool onspringCanConnect = AsyncHelper.RunTask(() => onspringAPI._client.CanConnectAsync());
            
            // create breaking bad api client.
            const string breakingBadBaseUrl = "https://www.breakingbadapi.com/api/";
            var breakingBadApi = new BreakingBadHelper(breakingBadBaseUrl);
            Console.WriteLine("Connecting to Breaking Bad API...");
            Console.WriteLine();

            // verify connectivity to breaking bad api.
            bool breakingBadCanConnect = breakingBadApi.CanConnect();

            // verify connectivity to breaking bad api and onspring api and log an appropriate response given responses received.
            if(onspringCanConnect && breakingBadCanConnect) { Console.WriteLine("Connected successfully."); }
            else if(!onspringCanConnect && breakingBadCanConnect) { Console.WriteLine("Could not connect to the Onspring API."); }
            else if(onspringCanConnect && !breakingBadCanConnect) { Console.WriteLine("Could not connect to the Breaking Bad API."); }
            else { Console.WriteLine("Unable to connect to either the Onspring API or the Breaking Bad API.");}
            Console.WriteLine();
            
            // load characters from thebreakingbadapi.com
            Console.WriteLine("Retrieving characters from thebreakingbadapi.com...");
            // var breakingBadCharacters = breakingBadApi.GetACharacterById(19);
            var breakingBadCharacters = breakingBadApi.GetAllCharacters();
            Console.WriteLine();

            if (breakingBadCharacters != null && breakingBadCharacters.Length > 0)
            {
                foreach(var breakingBadCharacter in breakingBadCharacters)
                {
                    var onspringCharacter = onspringAPI.GetCharacterById(breakingBadCharacter.char_id.ToString());

                    if(onspringCharacter != null)
                    {
                        Console.WriteLine("Found {0} in Onspring. (record id:{1})", onspringCharacter.name, onspringCharacter.recordId);
                    }
                    else
                    {
                        Console.WriteLine("Didn't find {0} in Onspring.", breakingBadCharacter.name);

                        var occupationRecordIds = onspringAPI.GetOccupationsByNameOrAddOccupations(breakingBadCharacter.occupation);
                        var seasonRecordIds = onspringAPI.GetSeasonsByNameOrAddSeasons(breakingBadCharacter.appearance);
                        var categoryRecordIds = onspringAPI.GetCategoriesByNameOrAddCategories(breakingBadCharacter.category);
                        var status = onspringAPI.GetStatusGuidValueByNameOrAddStatusListValue(breakingBadCharacter.status);
                        var birthday = DateTime.TryParse(breakingBadCharacter.birthday, out DateTime bday);

                        onspringCharacter = new OnspringCharacter
                        {
                            id = breakingBadCharacter.char_id,
                            name = breakingBadCharacter.name,
                            birthday = birthday ? bday : null,
                            occupation = occupationRecordIds,
                            status = status,
                            nickname = breakingBadCharacter.nickname,
                            appearances = seasonRecordIds,
                            portrayed = breakingBadCharacter.portrayed,
                            category = categoryRecordIds
                        };

                        var newCharacterRecordId = onspringAPI.AddNewOnspringCharacter(onspringCharacter);
                        if(newCharacterRecordId.HasValue)
                        {
                            Console.WriteLine("Added {0} in Onspring. (record id {1}) ", onspringCharacter.name, newCharacterRecordId);
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Failed to create {0} in Onspring.", breakingBadCharacter.name);
                        }
                    }
                }
            }
        }
    }
}
