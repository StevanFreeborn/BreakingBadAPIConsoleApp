using System;
using System.Configuration;
using Onspring.API.SDK.Helpers;
using Serilog;

namespace consoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.AppSettings().CreateLogger();

            // create onspring api client.
            var onspringBaseUrl = ConfigurationManager.AppSettings["baseUrl"]; ;
            var apiKey = ConfigurationManager.AppSettings["apiKey"];
            var onspringAPI = new OnspringHelper(onspringBaseUrl, apiKey);
            var onspringApiMessage = "Connecting to Onspring API...";
            Log.Information(onspringApiMessage);
            Console.WriteLine(onspringApiMessage);

            // verify connectivity to onspring api.
            bool onspringCanConnect = AsyncHelper.RunTask(() => onspringAPI.client.CanConnectAsync());
            
            // create breaking bad api client.
            const string breakingBadBaseUrl = "https://www.breakingbadapi.com/api/";
            var breakingBadApi = new BreakingBadHelper(breakingBadBaseUrl);
            var breakingBadApiMessage = "Connecting to Breaking Bad API...";
            Log.Information(breakingBadApiMessage);
            Console.WriteLine(breakingBadApiMessage);

            // verify connectivity to breaking bad api.
            bool breakingBadCanConnect = breakingBadApi.CanConnect();

            var connectionResultMessage = "";

            // verify connectivity to breaking bad api and onspring api and log an appropriate response given responses received.
            if(onspringCanConnect && breakingBadCanConnect)
            {
                connectionResultMessage = "Connected successfully.";
                Log.Information(connectionResultMessage);
                Console.WriteLine(connectionResultMessage);
            }
            else if(!onspringCanConnect && breakingBadCanConnect)
            {
                connectionResultMessage = "Could not connect to the Onspring API.";
                Log.Error(connectionResultMessage);
                throw new Exception(connectionResultMessage);
            }
            else if(onspringCanConnect && !breakingBadCanConnect)
            {
                connectionResultMessage = "Could not connect to the Breaking Bad API.";
                Log.Error(connectionResultMessage);
                throw new Exception(connectionResultMessage);
            }
            else
            {
                connectionResultMessage = "Unable to connect to either the Onspring API or the Breaking Bad API.";
                Log.Error(connectionResultMessage);
                throw new Exception(connectionResultMessage);
            }
            Console.WriteLine();

            // load characters from thebreakingbadapi.com.
            var retrievingCharacterMessage = "Retrieving character from thebreakingbadapi.com...";
            Log.Information(retrievingCharacterMessage);
            Console.WriteLine(retrievingCharacterMessage);
            var breakingBadCharacters = breakingBadApi.GetARandomCharacter();

            // check whether breaking bad characters were returned from request.
            if (breakingBadCharacters != null && breakingBadCharacters.Length > 0)
            {
                // loop through the breaking bad characters returned.
                foreach(var breakingBadCharacter in breakingBadCharacters)
                {
                    // get a character from onspring that has the same character id as the breaking bad character.
                    var onspringCharacter = onspringAPI.GetCharacterById(breakingBadCharacter.char_id.ToString());

                    // check to see if an onspring character was returned.
                    if(onspringCharacter != null)
                    {
                        var characterFoundMessage = string.Format("Found {0} in Onspring. (record id:{1})", onspringCharacter.name, onspringCharacter.recordId);
                        Log.Information(characterFoundMessage);
                        Console.WriteLine(characterFoundMessage);
                        Console.WriteLine();
                        var retrievingQuoteMessage = string.Format("Retrieving quotes by {0} from thebreakingbadapi.com...", breakingBadCharacter.name);
                        Log.Information(retrievingQuoteMessage);
                        Console.WriteLine(retrievingQuoteMessage);

                        var breakingBadCharacterQuotes = breakingBadApi.GetQuotesByAuthor(breakingBadCharacter.name);

                        if (breakingBadCharacterQuotes != null && breakingBadCharacterQuotes.Length > 0)
                        {
                            foreach (var quote in breakingBadCharacterQuotes)
                            {
                                onspringAPI.GetQuoteByIdOrAddQuote(quote, onspringCharacter.recordId);
                            }
                        }
                        else
                        {
                            var noQuoteFoundMessage = string.Format("No quotes found by {0}.", breakingBadCharacter.name);
                            Log.Information(noQuoteFoundMessage);
                            Console.WriteLine(noQuoteFoundMessage);
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        var noCharacterFoundMessage = string.Format("Didn't find {0} in Onspring.", breakingBadCharacter.name);
                        Log.Information(noCharacterFoundMessage);
                        Console.WriteLine(noCharacterFoundMessage);

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
                            var successfulCharacterAddMessage = string.Format("Added {0} in Onspring. (record id {1}) ", onspringCharacter.name, newCharacterRecordId);
                            Log.Information(successfulCharacterAddMessage);
                            Console.WriteLine(successfulCharacterAddMessage);
                            Console.WriteLine();

                            var retrievingQuoteMessage = string.Format("Retrieving quotes by {0} from thebreakingbadapi.com...", breakingBadCharacter.name);
                            Log.Information(retrievingQuoteMessage);
                            Console.WriteLine(retrievingQuoteMessage);

                            var breakingBadCharacterQuotes = breakingBadApi.GetQuotesByAuthor(breakingBadCharacter.name);

                            if (breakingBadCharacterQuotes != null && breakingBadCharacterQuotes.Length > 0)
                            {
                                foreach (var quote in breakingBadCharacterQuotes)
                                {
                                    onspringAPI.GetQuoteByIdOrAddQuote(quote, newCharacterRecordId);
                                }
                            }
                            else
                            {
                                var noQuoteFoundMessage = string.Format("No quotes found by {0}.", breakingBadCharacter.name);
                                Log.Information(noQuoteFoundMessage);
                                Console.WriteLine(noQuoteFoundMessage);
                                Console.WriteLine();
                            }

                        }
                        else
                        {
                            var unsuccessfulCharacterAddMessage = string.Format("Failed to create {0} in Onspring.", breakingBadCharacter.name);
                            Log.Information(unsuccessfulCharacterAddMessage);
                            Console.WriteLine(unsuccessfulCharacterAddMessage);
                        }
                    }
                }
            }
            Log.CloseAndFlush();
        }
    }
}
