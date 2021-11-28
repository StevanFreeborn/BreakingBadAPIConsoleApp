using System;
using System.Collections.Generic;

namespace consoleApplication
{
	public class BreakingBadCharacter
	{
		public int char_id { get; set; }
		public string name { get; set; }
		public string birthday { get; set; }
		public string[] occupation { get; set; }
		public string img { get; set; }
		public string status { get; set; }
		public string nickname { get; set; }
		public string[] appearance { get; set; }
		public string portrayed { get; set; }
		public string category { get; set; }
		public string better_call_saul_appearance { get; set; }

		public Guid? GetStatusGuidValue(string status)
        {
			if(status == "Alive") { return Guid.Parse("63afab90-b951-4b82-87c4-dfd20f8c98d9"); }
			if(status == "Deceased") { return Guid.Parse("41f8b40d-ae07-42d4-af53-9751dd88bb05"); }
			else { return null; }
        }
	}
}
