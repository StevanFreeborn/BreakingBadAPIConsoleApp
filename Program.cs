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
            Log.Information("Connecting to Onspring API...");

            // verify connectivity to onspring api.
            bool onspringCanConnect = AsyncHelper.RunTask(() => onspringAPI.client.CanConnectAsync());
            
            // create breaking bad api client.
            const string breakingBadBaseUrl = "https://www.breakingbadapi.com/api/";
            var breakingBadApi = new BreakingBadHelper(breakingBadBaseUrl);
            Log.Information("Connecting to Breaking Bad API...");

            // verify connectivity to breaking bad api.
            bool breakingBadCanConnect = breakingBadApi.CanConnect();

            var connectionResultMessage = "";

            // verify connectivity to breaking bad api and onspring api and log an appropriate response given responses received.
            if(onspringCanConnect && breakingBadCanConnect)
            {
                Log.Information("Connected successfully.");
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

            // load characters from thebreakingbadapi.com.
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
                        Log.Information("Found {onspringCharacterName} in Onspring. (record id:{recordId})", onspringCharacter.name, onspringCharacter.recordId);
                        
                        Log.Information("Retrieving quotes by {breakingBadCharacterName} from thebreakingbadapi.com...", breakingBadCharacter.name);
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
                            Log.Information("No quotes found by {breakingBadCharacterName}.", breakingBadCharacter.name);
                        }
                    }
                    else
                    {
                        Log.Information("Didn't find {breakingBadCharacterName} in Onspring.", breakingBadCharacter.name);

                        var occupationRecordIds = onspringAPI.GetOccupationsByNameOrAddOccupations(breakingBadCharacter.occupation);
                        var seasonRecordIds = onspringAPI.GetSeasonsByNameOrAddSeasons(breakingBadCharacter.appearance);
                        var categoryRecordIds = onspringAPI.GetCategoriesByNameOrAddCategories(breakingBadCharacter.category);
                        var status = onspringAPI.GetStatusGuidValueByNameOrAddStatusListValue(breakingBadCharacter.status);
                        var birthday = DateTime.TryParse(breakingBadCharacter.birthday, out DateTime bday);
                        onspringCharacter = new OnspringCharacter
                        {
                            id = breakingBadCharacter.char_id,
                            name = breakingBadCharacter.name,
                            birthday = birthday ? bday.ToUniversalTime() : null,
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
                            Log.Information("Added {onspringCharacterName} in Onspring. (record id {onspringCharacterRecordId})", onspringCharacter.name, newCharacterRecordId);

                            Log.Information("Retrieving quotes by {breakingBadCharacterName} from thebreakingbadapi.com...", breakingBadCharacter.name);
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
                                Log.Information("No quotes found by {breakingBadCharacterName}.", breakingBadCharacter.name);
                            }

                        }
                        else
                        {
                            Log.Error("Failed to create {breakingBadCharacterName} in Onspring.", breakingBadCharacter.name);
                        }
                    }
                }
            }
            Log.CloseAndFlush();
        }
    }
}
