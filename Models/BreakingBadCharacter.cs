using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace consoleApplication
{
	[Serializable]
	public class BreakingBadCharacter
	{
		[JsonProperty("char_id")]
		public int char_id { get; set; }
		[JsonProperty("name")]
		public string name { get; set; }
		[JsonProperty("birthday")]
		public string birthday { get; set; }
		[JsonProperty("occupation")]
		public List<string> occupation { get; set; }
		[JsonProperty("img")]
		public string img { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
		[JsonProperty("nickname")]
        public string nickname { get; set; }
		[JsonProperty("appearance")]
		[JsonConverter(typeof(CustomArrayConverter<string>))]
		public List<string> appearance { get; set; }
		[JsonProperty("portrayed")]
		public string portrayed { get; set; }
		[JsonProperty("category")]
		public string category { get; set; }
		[JsonProperty("better_call_saul_appearance")]
		[JsonConverter(typeof(CustomArrayConverter<string>))]
		public List<string> better_call_saul_appearance { get; set; }
	}
}
