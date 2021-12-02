using System.Configuration;
using Onspring.API.SDK.Models;

namespace consoleApplication
{
	public class OnspringCategoryMapper
	{
		public OnspringCategoryMapper()
		{
			categoriesAppId = int.Parse(ConfigurationManager.AppSettings[nameof(categoriesAppId)]);
			categoriesNameFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(categoriesNameFieldId)]);
			categoriesCharactersFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(categoriesCharactersFieldId)]);
			categoriesCharacterQuotesFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(categoriesCharacterQuotesFieldId)]);
		}

		public int categoriesAppId { get; }
		public int categoriesNameFieldId { get; }
		public int categoriesCharactersFieldId { get; }
		public int categoriesCharacterQuotesFieldId { get; }
		public OnspringCategory LoadCategory(ResultRecord record)
		{

			return new OnspringCategory
			{
				recordId = record.RecordId,
				Name = record.FieldData.Find(x => x.FieldId == categoriesNameFieldId).AsString(),
				Characters = record.FieldData.Find(x => x.FieldId == categoriesCharactersFieldId).AsIntegerList(),
				Quotes = record.FieldData.Find(x => x.FieldId == categoriesCharacterQuotesFieldId).AsIntegerList(),
			};
		}
		public ResultRecord GetAddEditCategoryValues(OnspringCategory category)
		{
			var record = new ResultRecord();
			record.AppId = categoriesAppId;
			record.RecordId = category.recordId;
			record.FieldData.Add(new StringFieldValue(categoriesNameFieldId, category.Name));
			record.FieldData.Add(new IntegerListFieldValue(categoriesCharactersFieldId, category.Characters));
			record.FieldData.Add(new IntegerListFieldValue(categoriesCharacterQuotesFieldId, category.Quotes));
			return record;
		}
	}
}
