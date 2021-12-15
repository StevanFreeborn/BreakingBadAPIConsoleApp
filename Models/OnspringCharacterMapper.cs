using System.Configuration;
using Onspring.API.SDK.Models;

namespace consoleApplication
{
	public class OnspringCharacterMapper
	{
		public OnspringCharacterMapper()
		{
			charactersAppId = int.Parse(ConfigurationManager.AppSettings[nameof(charactersAppId)]);
			charactersIdFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(charactersIdFieldId)]);
			charactersNameFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(charactersNameFieldId)]);
			charactersBirthdayFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(charactersBirthdayFieldId)]);
			charactersOccupationFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(charactersOccupationFieldId)]);
			charactersStatusFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(charactersStatusFieldId)]);
			charactersNicknameFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(charactersNicknameFieldId)]);
			charactersAppearanceFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(charactersAppearanceFieldId)]);
			charactersPotrayedFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(charactersPotrayedFieldId)]);
			characterCategoryFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(characterCategoryFieldId)]);
			characterQuotesFieldId = int.Parse(ConfigurationManager.AppSettings[nameof(characterQuotesFieldId)]);
			characterImageFieldId = int.Parse(ConfigurationManager.AppSettings.Get(nameof(characterImageFieldId)));
		}

		public int charactersAppId { get; }
		public int charactersIdFieldId { get; }
		public int charactersNameFieldId { get; }
		public int charactersBirthdayFieldId { get; }
		public int charactersOccupationFieldId { get; }
		public int charactersStatusFieldId { get; }
		public int charactersNicknameFieldId { get; }
		public int charactersAppearanceFieldId { get; }
		public int charactersPotrayedFieldId { get; }
		public int characterCategoryFieldId { get; }
		public int characterQuotesFieldId { get; }
		public int characterImageFieldId { get; }

		public OnspringCharacter LoadCharacter(ResultRecord record)
		{

			return new OnspringCharacter
			{
				recordId = record.RecordId,
				id = record.FieldData.Find(x => x.FieldId == charactersIdFieldId).AsNullableDecimal(),
				name = record.FieldData.Find(x => x.FieldId == charactersNameFieldId).AsString(),
				birthday = record.FieldData.Find(x => x.FieldId == charactersBirthdayFieldId).AsNullableDateTime(),
				occupation = record.FieldData.Find(x => x.FieldId == charactersOccupationFieldId).AsIntegerList(),
				status = record.FieldData.Find(x => x.FieldId == charactersStatusFieldId).AsNullableGuid(),
				nickname = record.FieldData.Find(x => x.FieldId == charactersNicknameFieldId).AsString(),
				appearances = record.FieldData.Find(x => x.FieldId == charactersAppearanceFieldId).AsIntegerList(),
				portrayed = record.FieldData.Find(x => x.FieldId == charactersPotrayedFieldId).AsString(),
				category = record.FieldData.Find(x => x.FieldId == characterCategoryFieldId).AsIntegerList()
			};
		}
		public ResultRecord GetAddEditCharacterValues(OnspringCharacter character)
		{
			var record = new ResultRecord();
			record.AppId = charactersAppId;
			record.RecordId = character.recordId;
			record.FieldData.Add(new DecimalFieldValue(charactersIdFieldId,character.id));
			record.FieldData.Add(new StringFieldValue(charactersNameFieldId,character.name));
			record.FieldData.Add(new DateFieldValue(charactersBirthdayFieldId,character.birthday));
			record.FieldData.Add(new IntegerListFieldValue(charactersOccupationFieldId,character.occupation));
			record.FieldData.Add(new GuidFieldValue(charactersStatusFieldId,character.status));
			record.FieldData.Add(new StringFieldValue(charactersNicknameFieldId,character.nickname));
			record.FieldData.Add(new IntegerListFieldValue(charactersAppearanceFieldId,character.appearances));
			record.FieldData.Add(new StringFieldValue(charactersPotrayedFieldId,character.portrayed));
			record.FieldData.Add(new IntegerListFieldValue(characterCategoryFieldId,character.category));
			return record;
		}
	}
}
