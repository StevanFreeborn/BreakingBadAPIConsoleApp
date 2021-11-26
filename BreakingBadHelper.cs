using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace consoleApplication
{
	public class BreakingBadHelper
	{
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
			else
			{
				return false;
			}
		}
		public BreakingBadCharacter[] GetAllCharacters()
		{
			var endPoint = "characters";
			var request = new RestRequest(endPoint);
			var response = _client.Get(request);
			var characters = JsonConvert.DeserializeObject<BreakingBadCharacter[]>(response.Content);
			return characters;
		}
		public BreakingBadCharacter[] GetACharactersByCategory(string Category)
        {
			var cat = Category.Replace(" ","+");
			var endPoint = "characters?category" + cat;
			var request = new RestRequest(endPoint);
			var response = _client.Get(request);
			var characters = JsonConvert.DeserializeObject<BreakingBadCharacter[]>(response.Content);
			return characters;
		}
		public BreakingBadCharacter[] GetACharacterById(int characterId)
		{
			var charId = characterId.ToString();
			var endPoint = "characters/" + charId;
			var request = new RestRequest(endPoint);
			var response = _client.Get(request);
			var characters = JsonConvert.DeserializeObject<BreakingBadCharacter[]>(response.Content);
			return characters;
		}
		public BreakingBadCharacter[] GetACharacterByName(string characterName)
		{
			var charName = characterName.Replace(" ", "+");
			var endPoint = "characters?=name" + charName;
			var request = new RestRequest(endPoint);
			var response = _client.Get(request);
			var characters = JsonConvert.DeserializeObject<BreakingBadCharacter[]>(response.Content);
			return characters;
		}
		public BreakingBadCharacter[] GetARandomCharacter()
		{
			var endPoint = "character/random";
			var request = new RestRequest(endPoint);
			var response = _client.Get(request);
			var characters = JsonConvert.DeserializeObject<BreakingBadCharacter[]>(response.Content);
			return characters;
		}
		public BreakingBadQuote[] GetQuotesByAuthor(string authorName)
        {
			var authName = authorName.Replace(" ", "+");
			var endPoint = "quote?author=" + authName;
			var request = new RestRequest(endPoint);
			var response = _client.Get(request);
			var quotes = JsonConvert.DeserializeObject<BreakingBadQuote[]>(response.Content);
			return quotes;
		}
	}
}