using System.Collections.Generic;
using Code.Core.ResourceSystem;

namespace Code.World.GUI.Converters;

public class MilMo_ConverterCategoryInfo
{
	public class ConverterCategoryInfoData
	{
		private readonly string _categoryIdentifier;

		private readonly MilMo_LocString _categoryDisplayName;

		private readonly string _openTexturePath;

		public string Identifier => _categoryIdentifier;

		public string OpenTexturePath => _openTexturePath;

		public MilMo_LocString DisplayName => _categoryDisplayName;

		public ConverterCategoryInfoData(string identifier, MilMo_LocString displayName, string openTexture)
		{
			_openTexturePath = openTexture;
			_categoryIdentifier = identifier;
			_categoryDisplayName = displayName;
		}
	}

	private static Dictionary<string, ConverterCategoryInfoData> _categories;

	private static readonly bool _dataRead;

	public static ConverterCategoryInfoData GetData(string identifier)
	{
		if (!_categories.ContainsKey(identifier))
		{
			return null;
		}
		return _categories[identifier];
	}

	public static void ReadData()
	{
		if (_dataRead)
		{
			return;
		}
		_categories = new Dictionary<string, ConverterCategoryInfoData>();
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal("GUI/Bag/ConverterCategories");
		if (milMo_SFFile == null)
		{
			return;
		}
		while (milMo_SFFile.NextRow())
		{
			string @string = milMo_SFFile.GetString();
			MilMo_LocString locString = MilMo_Localization.GetLocString(milMo_SFFile.GetString());
			string string2 = milMo_SFFile.GetString();
			if (!_categories.ContainsKey(@string))
			{
				_categories.Add(@string, new ConverterCategoryInfoData(@string, locString, string2));
			}
		}
	}
}
