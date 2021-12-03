using System.Configuration;
using Onspring.API.SDK.Models;

namespace consoleApplication
{
	public class OnspringQuoteMapper
	{
		public OnspringQuoteMapper()
		{
			quotesAppId = int.Parse(ConfigurationManager.AppSettings[nameof(quotesAppId)]);
			quotesIdFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(quotesIdFieldId)]);
			quotesQuoteFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(quotesQuoteFieldId)]);
			quotesAuthorFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(quotesAuthorFieldId)]);
			quotesSeriesFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(quotesSeriesFieldId)]);
		}
		public int quotesAppId { get; }
		public int quotesIdFieldId { get; }
		public int quotesQuoteFieldId { get; }
		public int quotesAuthorFieldId { get; }
		public int quotesSeriesFieldId { get; }

		public OnspringQuote LoadQuote(ResultRecord record)
		{

			return new OnspringQuote
			{
				recordId = record.RecordId,
				id = record.FieldData.Find(x => x.FieldId == quotesIdFieldId).AsNullableDecimal(),
				quote = record.FieldData.Find(x => x.FieldId == quotesQuoteFieldId).AsString(),
				author = record.FieldData.Find(x => x.FieldId == quotesAuthorFieldId).AsIntegerList(),
				series = record.FieldData.Find(x => x.FieldId == quotesSeriesFieldId).AsIntegerList(),
			};
		}
		public ResultRecord GetAddEditQuoteValues(OnspringQuote quote)
		{
			var record = new ResultRecord();
			record.AppId = quotesAppId;
			record.RecordId = quote.recordId;
			record.FieldData.Add(new DecimalFieldValue(quotesIdFieldId, quote.id));
			record.FieldData.Add(new StringFieldValue(quotesQuoteFieldId, quote.quote));
			record.FieldData.Add(new IntegerListFieldValue(quotesAuthorFieldId, quote.author));
			record.FieldData.Add(new IntegerListFieldValue(quotesSeriesFieldId, quote.series));
			return record;
		}
	}
}
