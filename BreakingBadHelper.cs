using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace consoleApplication
{
	public class BreakingBadHelper
	{
		public OnspringCharacterMapper _mapper = new OnspringCharacterMapper();
		public RestClient _client;
		public BreakingBadHelper(string apiUrl)
		{
			_client = new RestClient(apiUrl);
		}
		public bool CanConnect()
        {
			var request = new RestRequest();
			var response = _client.Get(request);
			var status = response.IsSuccessful;
			if (status)
            {
				return true;
            }
			return false;
        }
		public BreakingBadCharacter[] GetAllCharacters()
        {
			var request = new RestRequest("characters");
			var response = _client.Get(request);
			var characters = JsonConvert.DeserializeObject<BreakingBadCharacter[]>(response.Content);
			return characters;
        }
		public BreakingBadCharacter[] GetACharacter(int num)
        {
			var charId = num.ToString();
			var endPoint = "characters/" + charId;
			var request = new RestRequest(endPoint);
			var response = _client.Get(request);
			var character = JsonConvert.DeserializeObject<BreakingBadCharacter[]>(response.Content);
			return character;
		}
	}
}