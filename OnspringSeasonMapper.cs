using System;
using System.Configuration;
using System.Collections.Generic;
using Onspring.API.SDK.Models;

namespace consoleApplication
{
    public class OnspringSeasonMapper
    {
		public OnspringSeasonMapper()
		{
			seasonsAppId = int.Parse(ConfigurationManager.AppSettings[nameof(seasonsAppId)]);
			seasonsNameFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(seasonsNameFieldId)]);
			seasonsCharactersFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(seasonsCharactersFieldId)]);
		}

		public int seasonsAppId { get; }
		public int seasonsNameFieldId { get; }
		public int seasonsCharactersFieldId { get; }

		public OnspringSeason LoadSeason(ResultRecord record)
		{

			return new OnspringSeason
			{
				recordId = record.RecordId,
				Name = record.FieldData.Find(x => x.FieldId == seasonsNameFieldId).AsString(),
				Characters = record.FieldData.Find(x => x.FieldId == seasonsCharactersFieldId).AsIntegerList(),
			};
		}
		public ResultRecord GetAddEditSeasonValues(OnspringSeason season)
		{
			var record = new ResultRecord();
			record.AppId = seasonsAppId;
			record.RecordId = season.recordId;
			record.FieldData.Add(new StringFieldValue(seasonsNameFieldId, season.Name));
			record.FieldData.Add(new IntegerListFieldValue(seasonsCharactersFieldId, season.Characters));
			return record;
		}
	}
}
