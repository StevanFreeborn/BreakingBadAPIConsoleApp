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

		public object GetAllCharacters()
        {
			var request = new RestRequest("characters");
			var response = _client.Get(request);
			var thing = JsonConvert.DeserializeObject<object>(response.Content);
			return thing;
        }
		public object GetACharacter()
        {
			var endPoint = "character/1";
			var request = new RestRequest(endPoint);
			var response = _client.Get(request);
			var thing = JsonConvert.DeserializeObject<object>(response.Content);
			return thing;
		}
	}
}