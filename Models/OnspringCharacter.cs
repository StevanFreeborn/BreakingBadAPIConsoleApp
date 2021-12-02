using System;
using System.Collections.Generic;

namespace consoleApplication
{
	public class OnspringCharacter
	{
		public int recordId { get; set; }
		public decimal? id { get; set; }
		public string name { get; set; }
		public DateTime? birthday { get; set; }
		public List<int> occupation { get; set; }
		public Guid? status { get; set; }
		public string nickname { get; set; }
		public List<int> appearances { get; set; }
		public string portrayed { get; set; }
		public List<int> category { get; set; }

	}
}
