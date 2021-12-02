using System;
using System.Collections.Generic;

namespace consoleApplication
{
	public class OnspringCategory
	{
		public int recordId { get; set; }
		public string Name { get; set; }
		public List<int> Characters { get; set; }
		public List<int> Quotes { get; set; }
	}
}
