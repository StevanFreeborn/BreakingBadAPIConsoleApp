using System;
using Newtonsoft.Json;
using RestSharp;

namespace consoleApplication
{
	public class BreakingBadHelper
	{
		public OnspringCharacterMapper _mapper = new OnspringCharacterMapper()
		public RestClient _client;
		public BreakingBadHelper(string apiUrl)
		{
			_client = new RestClient(apiUrl);
		}
	}
}