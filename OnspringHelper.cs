using System;
using Onspring.API.SDK;
using Onspring.API.SDK.Helpers;

namespace consoleApplication
{
    public class OnspringHelper
    {
        public OnspringClient _client;

        public OnspringHelper(string baseUrl, string apiKey)
        {
            _client = new OnspringClient(baseUrl, apiKey);
        }
    }
}
