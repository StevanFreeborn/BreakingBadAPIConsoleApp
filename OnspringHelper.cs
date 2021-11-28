using System;
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

        public OnspringHelper(string baseUrl, string apiKey)
        {
            _client = new OnspringClient(baseUrl, apiKey);
            _characterMapper = new OnspringCharacterMapper();
            _occupationMapper = new OnspringOccupationMapper();
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
        public List<int?> GetOccupationRecordIds(string[] occupationNames)
        {
            var occupationRecordIds = new List<int?>();

            foreach (var name in occupationNames)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return null;
                }

                var queryRequest = new QueryRecordsRequest()
                {
                    AppId = _occupationMapper.occupationsAppId,
                    Filter = $"{_occupationMapper.occupationsNameFieldId} eq '{name}'",
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
                    occupationRecordIds.Add(newRecordId);
                }
            }
            return occupationRecordIds;
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
    }
}
