using System;
using System.Configuration;
using Onspring.API.SDK.Models;

namespace consoleApplication
{
	public class OnspringOccupationMapper
	{
		public OnspringOccupationMapper()
		{
			occupationsAppId = int.Parse(ConfigurationManager.AppSettings[nameof(occupationsAppId)]);
			occupationsNameFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(occupationsNameFieldId)]);
			occupationsCharactersFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(occupationsCharactersFieldId)]);
		}

		public int occupationsAppId { get; }
		public int occupationsNameFieldId { get; }
		public int occupationsCharactersFieldId { get; }

		public OnspringOccupation LoadOccupation(ResultRecord record)
		{

			return new OnspringOccupation
			{
				recordId = record.RecordId,
				Name = record.FieldData.Find(x => x.FieldId == occupationsNameFieldId).AsString(),
				Characters = record.FieldData.Find(x => x.FieldId == occupationsCharactersFieldId).AsIntegerList(),
			};
		}
		public ResultRecord GetAddEditOccupationValues(OnspringOccupation occupation)
		{
			var record = new ResultRecord();
			record.AppId = occupationsAppId;
			record.RecordId = occupation.recordId;
			record.FieldData.Add(new StringFieldValue(occupationsNameFieldId, occupation.Name));
			record.FieldData.Add(new IntegerListFieldValue(occupationsCharactersFieldId, occupation.Characters));
			return record;
		}
	}
}

