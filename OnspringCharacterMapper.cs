using System;
using System.Configuration;
using Onspring.API.SDK;
using Onspring.API.SDK.Helpers;
using Onspring.API.SDK.Enums;
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

		public OnspringCharacter LoadCharacter(ResultRecord record)
        {
			return null;
        }
	}
}
