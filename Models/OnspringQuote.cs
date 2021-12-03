using System.Collections.Generic;

namespace consoleApplication
{
	public class OnspringQuote
	{
		public int recordId { get; set; }
		public decimal? id { get; set; }
		public string quote { get; set; }
		public List<int> author { get; set; }
		public List<int> series { get; set; }
	}
}
