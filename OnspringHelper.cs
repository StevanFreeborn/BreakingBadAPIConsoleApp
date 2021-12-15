using System;
using System.IO;
using System.Collections.Generic;
using Onspring.API.SDK;
using Onspring.API.SDK.Models;
using Onspring.API.SDK.Helpers;
using Serilog;
using RestSharp;
using Newtonsoft.Json;
using System.Net.Http;

namespace consoleApplication
{
    public class OnspringHelper
    {
        public OnspringClient client;
        public OnspringCharacterMapper characterMapper;
        public OnspringOccupationMapper occupationMapper;
        public OnspringSeasonMapper seasonMapper;
        public OnspringCategoryMapper categoryMapper;
        public OnspringQuoteMapper quoteMapper;

        public OnspringHelper(string baseUrl, string apiKey)
        {
            client = new OnspringClient(baseUrl, apiKey);
            characterMapper = new OnspringCharacterMapper();
            occupationMapper = new OnspringOccupationMapper();
            seasonMapper = new OnspringSeasonMapper();
            categoryMapper = new OnspringCategoryMapper();
            quoteMapper = new OnspringQuoteMapper();
        }

        public class saveFileResponse
        {
            public int? id { get; set; }
        }


        public OnspringCharacter GetCharacterById(string characterId)
        {
            if (string.IsNullOrEmpty(characterId))
            {
                return null;
            }
            var queryRequest = new QueryRecordsRequest
            {
                AppId = characterMapper.charactersAppId,
                Filter = $"{characterMapper.charactersIdFieldId} eq {characterId}",
                FieldIds = new List<int> { },
                DataFormat = Onspring.API.SDK.Enums.DataFormat.Raw,
            };

            Log.Debug("GetCharacterById Request: {@queryRequest}", queryRequest);
            var queryResponse = AsyncHelper.RunTask(() => client.QueryRecordsAsync(queryRequest));
            Log.Debug("GetCharacterById Response: {@queryResponse}", queryResponse);

            var records = queryResponse.Value.Items;

            switch (records.Count)
            {
                case 0:
                    return null;
                case 1:
                    return characterMapper.LoadCharacter(records[0]);
                default:
                    var errorMessage = "More than one character with id: Yes " + characterId;
                    Log.Error(errorMessage);
                    throw new Exception(errorMessage);
            }
        }
        public List<int> GetOccupationsByNameOrAddOccupations(List<string> occupationNames)
        {
            var occupationRecordIds = new List<int>();

            foreach (var name in occupationNames)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return null;
                }

                var filterValue = name.Replace("'", "''");

                var queryRequest = new QueryRecordsRequest()
                {
                    AppId = occupationMapper.occupationsAppId,
                    Filter = $"{occupationMapper.occupationsNameFieldId} eq '{filterValue}'",
                    FieldIds = new List<int> { },
                    DataFormat = Onspring.API.SDK.Enums.DataFormat.Raw,
                };

                Log.Debug("GetOccupationsByNameOrAddOccupations Request: {@queryRequest}", queryRequest);
                var queryResponse = AsyncHelper.RunTask(() => client.QueryRecordsAsync(queryRequest));
                Log.Debug("GetOccupationsByNameOrAddOccupations Response: {@queryResponse}", queryResponse);

                var records = queryResponse.Value.Items;
                if (records.Count > 0)
                {
                    var onspringOccupation = occupationMapper.LoadOccupation(records[0]);
                    occupationRecordIds.Add(onspringOccupation.recordId);
                }
                else
                {
                    var onspringOccupation = new OnspringOccupation
                    {
                        Name = name,
                    };

                    var newRecordId = AddNewOnspringOccupation(onspringOccupation);
                    occupationRecordIds.Add(newRecordId.GetValueOrDefault());
                }
            }
            return occupationRecordIds;
        }
        public List<int> GetSeasonsByNameOrAddSeasons(List<string> seasonNames)
        {
            var seasonRecordIds = new List<int>();

            foreach (var name in seasonNames)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return null;
                }

                var filterValue = name.Replace("'", "''");

                var queryRequest = new QueryRecordsRequest()
                {
                    AppId = seasonMapper.seasonsAppId,
                    Filter = $"{seasonMapper.seasonsNameFieldId} eq '{filterValue}'",
                    FieldIds = new List<int> { },
                    DataFormat = Onspring.API.SDK.Enums.DataFormat.Raw,
                };

                Log.Debug("GetOccupationsByNameOrAddOccupations Request: {@queryRequest}", queryRequest);
                var queryResponse = AsyncHelper.RunTask(() => client.QueryRecordsAsync(queryRequest));
                Log.Debug("GetOccupationsByNameOrAddOccupations Response: {@queryResponse}", queryResponse);

                var records = queryResponse.Value.Items;
                if (records.Count > 0)
                {
                    var onspringSeason = seasonMapper.LoadSeason(records[0]);
                    seasonRecordIds.Add(onspringSeason.recordId);
                }
                else
                {
                    var onspringSeason = new OnspringSeason
                    {
                        Name = name,
                    };

                    var newRecordId = AddNewOnspringSeason(onspringSeason);
                    seasonRecordIds.Add(newRecordId.GetValueOrDefault());
                }
            }
            return seasonRecordIds;
        }
        public List<int> GetCategoriesByNameOrAddCategories(string categories)
        {
            var splitCategories = new List<string>(categories.Split(","));
            List<string> trimmedCategories = new List<string>();
            foreach (var category in splitCategories)
            {
                trimmedCategories.Add(category.Trim());
            }

            var categoryRecordIds = new List<int>();

            foreach (var category in trimmedCategories)
            {
                if (string.IsNullOrEmpty(category))
                {
                    return null;
                }

                var filterValue = category.Replace("'", "''");

                var queryRequest = new QueryRecordsRequest()
                {
                    AppId = categoryMapper.categoriesAppId,
                    Filter = $"{categoryMapper.categoriesNameFieldId} eq '{filterValue}'",
                    FieldIds = new List<int> { },
                    DataFormat = Onspring.API.SDK.Enums.DataFormat.Raw,
                };

                Log.Debug("GetOccupationsByNameOrAddOccupations Request: {@queryRequest}", queryRequest);
                var queryResponse = AsyncHelper.RunTask(() => client.QueryRecordsAsync(queryRequest));
                Log.Debug("GetOccupationsByNameOrAddOccupations Response: {@queryResponse}", queryResponse);

                var records = queryResponse.Value.Items;
                if (records.Count > 0)
                {
                    var onspringCategory = categoryMapper.LoadCategory(records[0]);
                    categoryRecordIds.Add(onspringCategory.recordId);
                }
                else
                {
                    var onspringCategory = new OnspringCategory
                    {
                        Name = category,
                    };

                    var newRecordId = AddNewOnspringCategory(onspringCategory);
                    categoryRecordIds.Add(newRecordId.GetValueOrDefault());
                }
            }
            return categoryRecordIds;
        }
        public Guid? GetStatusGuidValueByNameOrAddStatusListValue(string status)
        {
            var statusFieldId = characterMapper.charactersStatusFieldId;
            var getFieldResponse = client.GetFieldAsync(statusFieldId);

            Log.Debug("getFieldResponse: {@getFieldResponse}", getFieldResponse);

            var field = getFieldResponse.Result.Value;
            var statusListField = field as ListField;
            var listId = statusListField.ListId;
            Guid? statusGuidValue = null;

            foreach (var item in statusListField.Values)
            {
                if (item.Name.ToLower() == status.ToLower())
                {
                    statusGuidValue = item.Id;
                }
            }
            if (statusGuidValue != null)
            {
                return statusGuidValue;
            }
            else
            {
                var saveListItemRequest = new SaveListItemRequest
                {
                    ListId = listId,
                    Id = null,
                    Name = status,
                    NumericValue = null,
                    Color = null,
                };

                Log.Debug("saveListItemRequest: {@saveListItemRequest}", saveListItemRequest);
                var saveListItemResponse = client.SaveListItemAsync(saveListItemRequest);
                Log.Debug("saveListItemResponse: {@saveListItemResponse}", saveListItemResponse);

                return saveListItemResponse.Result.Value.Id;
            }
        }
        public void GetQuoteByIdOrAddQuote(BreakingBadQuote quote, int? onspringCharacterRecordId)
        {
            if (string.IsNullOrEmpty(quote.quote_id.ToString()))
            {
                Log.Information("Could not look up or add quote {quoteId} in Onspring.", quote.quote_id);
            }
            var queryRequest = new QueryRecordsRequest
            {
                AppId = quoteMapper.quotesAppId,
                Filter = $"{quoteMapper.quotesIdFieldId} eq {quote.quote_id}",
                FieldIds = new List<int> { },
                DataFormat = Onspring.API.SDK.Enums.DataFormat.Raw,
            };

            Log.Debug("GetQuoteByIdOrAddQuote Request: {@queryRequest}", queryRequest);
            var queryResponse = AsyncHelper.RunTask(() => client.QueryRecordsAsync(queryRequest));
            Log.Debug("GetQuoteByIdOrAddQuote Response: {@queryResponse}", queryResponse);

            var records = queryResponse.Value.Items;

            if (records.Count > 1)
            {
                var errorMessage = "More than one quote with id: " + quote.quote_id;
                Log.Error(errorMessage);
                throw new ApplicationException(errorMessage);
            }
            else if (records.Count > 0)
            {
                var onspringQuote = quoteMapper.LoadQuote(records[0]);
                Log.Information("Found quote {breakingBadQuoteQuoteId} in Onspring. (record id:{onspringQuoteRecordId})", quote.quote_id, onspringQuote.recordId);
            }
            else
            {
                var author = new List<int>();
                author.Add(onspringCharacterRecordId.GetValueOrDefault());
                var series = GetCategoriesByNameOrAddCategories(quote.series);
                var onspringQuote = new OnspringQuote
                {
                    id = quote.quote_id,
                    quote = quote.quote,
                    author = author,
                    series = series
                };
                var newOnspringQuoteRecordId = AddNewOnspringQuote(onspringQuote);
                if (newOnspringQuoteRecordId.HasValue)
                {
                    Log.Information("Added quote {onspringQuoteId} in Onspring. (record id {onspringQuoteRecordId}) ", onspringQuote.id, newOnspringQuoteRecordId);
                }
                else
                {
                    Log.Error("Failed to create quote {breakingBadQuoteQuoteId} in Onspring.", quote.quote_id);
                }
            }
        }
        public int? AddNewOnspringCharacter(OnspringCharacter character)
        {
            var record = characterMapper.GetAddEditCharacterValues(character);
            var saveResponse = client.SaveRecordAsync(record);
            Log.Debug("AddNewOnspringCharacter saveResponse: {@saveResponse}", saveResponse);
            return saveResponse.Result.Value.Id;
        }
        public int? AddNewOnspringOccupation(OnspringOccupation occupation)
        {
            var record = occupationMapper.GetAddEditOccupationValues(occupation);
            var saveResponse = client.SaveRecordAsync(record);
            Log.Debug("AddNewOnspringOccupation saveResponse: {@saveResponse}", saveResponse);
            return saveResponse?.Result.Value.Id;
        }
        public int? AddNewOnspringSeason(OnspringSeason season)
        {
            var record = seasonMapper.GetAddEditSeasonValues(season);
            var saveResponse = client.SaveRecordAsync(record);
            Log.Debug("AddNewOnspringSeason saveResponse: {@saveResponse}", saveResponse);
            return saveResponse?.Result.Value.Id;
        }
        public int? AddNewOnspringCategory(OnspringCategory category)
        {
            var record = categoryMapper.GetAddEditCategoryValues(category);
            var saveResponse = client.SaveRecordAsync(record);
            Log.Debug("AddNewOnspringCategory saveResponse: {@saveResponse}", saveResponse);
            return saveResponse?.Result.Value.Id;
        }
        public int? AddNewOnspringQuote(OnspringQuote quote)
        {
            var record = quoteMapper.GetAddEditQuoteValues(quote);
            var saveResponse = client.SaveRecordAsync(record);
            Log.Debug("AddNewOnspringQuote saveResponse: {@saveResponse}", saveResponse);
            return saveResponse?.Result.Value.Id;
        }
        public int? AddOnspringCharacterImage(string imageUrl, int? characterRecordId)
        {
            var url = imageUrl;
            string filePath = null;

            using (var _client = new HttpClient())
            using (var file = _client.GetAsync(url))
            {
                file.Result.EnsureSuccessStatusCode();
                var mediaType = file.Result.Content.Headers.ContentType.MediaType;
                Log.Debug("Character image media type: {mediaType}", mediaType);
                var fileExtension = string.Join("", ".", mediaType.Substring(mediaType.LastIndexOf("/")+1));
                Log.Debug("Character image file extension: {fileExtension}", fileExtension);
                var guid = Guid.NewGuid().ToString();
                var fileName = string.Join("", "characterImage", guid, fileExtension);
                Log.Debug("Character image file name: {fileName}", fileName);
                var stream = file.Result.Content.ReadAsStreamAsync();

                filePath = Path.Combine("C:\\Software Projects\\OnspringApiV2\\src\\consoleApplication\\images", fileName);
                Log.Debug("Character image file path: {filePath}", filePath);

                using (var fileStream = File.Create(filePath))
                {
                    stream.Result.CopyTo(fileStream);
                }
            }

            var client = new RestClient("https://api.alpha.onspring.ist/Files");
            var request = new RestRequest(Method.POST);
            request.AddHeader("x-apikey", "61546a78a65cf5787573c39a/2b02fc67-e3c4-4292-8438-201e1ecec61d");
            request.AddHeader("x-apiversion", "2");
            request.AddParameter("RecordId", characterRecordId.ToString());
            request.AddParameter("FieldId", characterMapper.characterImageFieldId.ToString());
            request.AddParameter("Notes", "Adding character image");
            request.AddParameter("ModifiedDate", DateTime.UtcNow.ToString());
            request.AddFile("File", filePath);
            Log.Debug("AddOnspringCharacterImage Request: {@request}", request);
            IRestResponse response = client.Execute(request);
            Log.Debug("AddOnspringCharacterImage Request: {@response}", response);

            var json = JsonConvert.DeserializeObject<saveFileResponse>(response.Content);
            Log.Debug("{@json}", json);
            var fileId = json.id;

            if(fileId.HasValue)
            {
                Log.Information("Successfully added character's image. (file id: {fileId})", fileId);
                return fileId;
            }
            else
            {
                Log.Information("Unable to add character's image.");
                return null;
            }

        }
    }
}
