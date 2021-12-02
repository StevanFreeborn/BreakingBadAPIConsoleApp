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
        public OnspringClient _client;
        public OnspringCharacterMapper _characterMapper;
        public OnspringOccupationMapper _occupationMapper;
        public OnspringSeasonMapper _seasonMapper;
        public OnspringCategoryMapper _categoryMapper;

        public OnspringHelper(string baseUrl, string apiKey)
        {
            _client = new OnspringClient(baseUrl, apiKey);
            _characterMapper = new OnspringCharacterMapper();
            _occupationMapper = new OnspringOccupationMapper();
            _seasonMapper = new OnspringSeasonMapper();
            _categoryMapper = new OnspringCategoryMapper();
        }

        public OnspringCharacter GetCharacterById(string characterId)
        {
            if(string.IsNullOrEmpty(characterId))
            {
                return null;
            }
            var queryRequest = new QueryRecordsRequest
            {
                AppId = _characterMapper.charactersAppId,
                Filter = $"{_characterMapper.charactersIdFieldId} eq {characterId}",
                FieldIds = new List<int> {},
                DataFormat = DataFormat.Raw,
            };
            var queryResponse = AsyncHelper.RunTask(() => _client.QueryRecordsAsync(queryRequest));
            var records = queryResponse.Value.Items;

            switch (records.Count)
            {
                case 0:
                    return null;
                case 1:
                    return _characterMapper.LoadCharacter(records[0]);
                default:
                    throw new ApplicationException("More than one character with id: " + characterId);
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
                    AppId = _occupationMapper.occupationsAppId,
                    Filter = $"{_occupationMapper.occupationsNameFieldId} eq '{filterValue}'",
                    FieldIds = new List<int> { },
                    DataFormat = DataFormat.Raw,
                };

                var queryResponse = AsyncHelper.RunTask(() => _client.QueryRecordsAsync(queryRequest));
                var records = queryResponse.Value.Items;
                if(records.Count > 0)
                {
                    var onspringOccupation = _occupationMapper.LoadOccupation(records[0]);
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
                    AppId = _seasonMapper.seasonsAppId,
                    Filter = $"{_seasonMapper.seasonsNameFieldId} eq '{filterValue}'",
                    FieldIds = new List<int> { },
                    DataFormat = DataFormat.Raw,
                };

                var queryResponse = AsyncHelper.RunTask(() => _client.QueryRecordsAsync(queryRequest));
                var records = queryResponse.Value.Items;
                if (records.Count > 0)
                {
                    var onspringSeason = _seasonMapper.LoadSeason(records[0]);
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
                    AppId = _categoryMapper.categoriesAppId,
                    Filter = $"{_categoryMapper.categoriesNameFieldId} eq '{filterValue}'",
                    FieldIds = new List<int> { },
                    DataFormat = DataFormat.Raw,
                };

                var queryResponse = AsyncHelper.RunTask(() => _client.QueryRecordsAsync(queryRequest));
                var records = queryResponse.Value.Items;
                if (records.Count > 0)
                {
                    var onspringCategory = _categoryMapper.LoadCategory(records[0]);
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
            var statusFieldId = _characterMapper.charactersStatusFieldId;
            var getFieldResponse = _client.GetFieldAsync(statusFieldId);
            var field = getFieldResponse.Result.Value;
            var statusListField = field as ListField;
            var listId = int.Parse(ConfigurationManager.AppSettings["charactersStatusFieldListId"]);
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
                var saveListItemResponse = _client.SaveListItemAsync(saveListItemRequest);
                return saveListItemResponse.Result.Value.Id;
            }
        }
        public int? AddNewOnspringCharacter(OnspringCharacter character)
        {
            var record = _characterMapper.GetAddEditCharacterValues(character);
            var saveResponse = _client.SaveRecordAsync(record);
            return saveResponse.Result.Value.Id;
        }
        public int? AddNewOnspringOccupation(OnspringOccupation occupation)
        {
            var record = _occupationMapper.GetAddEditOccupationValues(occupation);
            var saveResponse = _client.SaveRecordAsync(record);
            return saveResponse?.Result.Value.Id;
        }
        public int? AddNewOnspringSeason(OnspringSeason season)
        {
            var record = _seasonMapper.GetAddEditSeasonValues(season);
            var saveResponse = _client.SaveRecordAsync(record);
            return saveResponse?.Result.Value.Id;
        }
        public int? AddNewOnspringCategory(OnspringCategory category)
        {
            var record = _categoryMapper.GetAddEditCategoryValues(category);
            var saveResponse = _client.SaveRecordAsync(record);
            return saveResponse?.Result.Value.Id;
        }
    }
}
