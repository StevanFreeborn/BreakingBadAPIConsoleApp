using System;
using System.Configuration;
using System.Collections.Generic;
using Onspring.API.SDK;
using Onspring.API.SDK.Models;
using Onspring.API.SDK.Enums;
using Onspring.API.SDK.Helpers;

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

        public OnspringCharacter GetCharacterById(string characterId)
        {
            if(string.IsNullOrEmpty(characterId))
            {
                return null;
            }
            var queryRequest = new QueryRecordsRequest
            {
                AppId = characterMapper.charactersAppId,
                Filter = $"{characterMapper.charactersIdFieldId} eq {characterId}",
                FieldIds = new List<int> {},
                DataFormat = DataFormat.Raw,
            };
            var queryResponse = AsyncHelper.RunTask(() => client.QueryRecordsAsync(queryRequest));
            var records = queryResponse.Value.Items;

            switch (records.Count)
            {
                case 0:
                    return null;
                case 1:
                    return characterMapper.LoadCharacter(records[0]);
                default:
                    throw new Exception("More than one character with id: " + characterId);
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
                    DataFormat = DataFormat.Raw,
                };

                var queryResponse = AsyncHelper.RunTask(() => client.QueryRecordsAsync(queryRequest));
                var records = queryResponse.Value.Items;
                if(records.Count > 0)
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
                    DataFormat = DataFormat.Raw,
                };

                var queryResponse = AsyncHelper.RunTask(() => client.QueryRecordsAsync(queryRequest));
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
            foreach(var category in splitCategories)
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
                    DataFormat = DataFormat.Raw,
                };

                var queryResponse = AsyncHelper.RunTask(() => client.QueryRecordsAsync(queryRequest));
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
            var field = getFieldResponse.Result.Value;
            var statusListField = field as ListField;
            var listId = statusListField.ListId;
            Guid? statusGuidValue = null;
            foreach(var item in statusListField.Values)
            {
                if(item.Name.ToLower() == status.ToLower())
                {
                    statusGuidValue = item.Id;
                }
            }
            if(statusGuidValue != null)
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
                var saveListItemResponse = client.SaveListItemAsync(saveListItemRequest);
                return saveListItemResponse.Result.Value.Id;
            }
        }
        public void GetQuoteByIdOrAddQuote(BreakingBadQuote quote, int? onspringCharacterRecordId)
        {
            if (string.IsNullOrEmpty(quote.quote_id.ToString()))
            {
                Console.WriteLine("Could not look up or add quote {0} in Onspring.", quote.quote_id);
            }
            var queryRequest = new QueryRecordsRequest
            {
                AppId = quoteMapper.quotesAppId,
                Filter = $"{quoteMapper.quotesIdFieldId} eq {quote.quote_id}",
                FieldIds = new List<int> { },
                DataFormat = DataFormat.Raw,
            };
            var queryResponse = AsyncHelper.RunTask(() => client.QueryRecordsAsync(queryRequest));
            var records = queryResponse.Value.Items;

            if(records.Count > 1)
            {
                throw new ApplicationException("More than one quote with id: " + quote.quote_id);
            }
            else if(records.Count > 0)
            {
                var onspringQuote = quoteMapper.LoadQuote(records[0]);
                Console.WriteLine("Found quote {0} in Onspring. (record id:{1})", quote.quote_id, onspringQuote.recordId);
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
                    Console.WriteLine("Added quote {0} in Onspring. (record id {1}) ", onspringQuote.id, newOnspringQuoteRecordId);
                }
                else
                {
                    Console.WriteLine("Failed to create quote {0} in Onspring.", quote.quote_id);
                }
            }
        }
        public int? AddNewOnspringCharacter(OnspringCharacter character)
        {
            var record = characterMapper.GetAddEditCharacterValues(character);
            var saveResponse = client.SaveRecordAsync(record);
            return saveResponse.Result.Value.Id;
        }
        public int? AddNewOnspringOccupation(OnspringOccupation occupation)
        {
            var record = occupationMapper.GetAddEditOccupationValues(occupation);
            var saveResponse = client.SaveRecordAsync(record);
            return saveResponse?.Result.Value.Id;
        }
        public int? AddNewOnspringSeason(OnspringSeason season)
        {
            var record = seasonMapper.GetAddEditSeasonValues(season);
            var saveResponse = client.SaveRecordAsync(record);
            return saveResponse?.Result.Value.Id;
        }
        public int? AddNewOnspringCategory(OnspringCategory category)
        {
            var record = categoryMapper.GetAddEditCategoryValues(category);
            var saveResponse = client.SaveRecordAsync(record);
            return saveResponse?.Result.Value.Id;
        }
        public int? AddNewOnspringQuote(OnspringQuote quote)
        {
            var record = quoteMapper.GetAddEditQuoteValues(quote);
            var saveResponse = client.SaveRecordAsync(record);
            return saveResponse?.Result.Value.Id;
        }
    }
}
