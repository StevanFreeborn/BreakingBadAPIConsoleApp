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
        public OnspringCharacterMapper _characterMapper = new OnspringCharacterMapper();

        public OnspringHelper(string baseUrl, string apiKey)
        {
            _client = new OnspringClient(baseUrl, apiKey);
        }

        public async OnspringCharacter GetCharacterById(string characterId)
        {
            if(string.IsNullOrEmpty(characterId))
            {
                return null;
            }
            var queryRequest = new QueryRecordsRequest
            {
                AppId = _characterMapper.charactersAppId,
                Filter = $"{_characterMapper.charactersIdFieldId} eq '{characterId}",
                FieldIds = new List<int> { },
                DataFormat = DataFormat.Formatted,
            };
            var queryResponse = await _client.QueryRecordsAsync(queryRequest);
            var records = queryResponse.Value.Items;
        }
    }
}
