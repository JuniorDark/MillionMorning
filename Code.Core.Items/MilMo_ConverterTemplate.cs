using System.Collections.Generic;
using Code.Core.Network.types;
using Code.World.GUI.Converters;

namespace Code.Core.Items;

public class MilMo_ConverterTemplate : MilMo_ItemTemplate
{
	public string ConverterCategory { get; protected set; }

	public int RequiredGems { get; protected set; }

	public TemplateReference RequiredTool { get; protected set; }

	public IList<TemplateCountPair> Ingredients { get; protected set; }

	public TemplateCountPair GirlReward { get; protected set; }

	public TemplateCountPair BoyReward { get; protected set; }

	public string OpenTexture
	{
		get
		{
			MilMo_ConverterCategoryInfo.ConverterCategoryInfoData data = MilMo_ConverterCategoryInfo.GetData(ConverterCategory);
			if (data == null)
			{
				return "";
			}
			return data.OpenTexturePath;
		}
	}

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		ConverterTemplate converterTemplate = t as ConverterTemplate;
		base.LoadFromNetwork((Code.Core.Network.types.Template)converterTemplate);
		if (converterTemplate == null)
		{
			return false;
		}
		ConverterCategory = converterTemplate.GetConverterCategory();
		Ingredients = new List<TemplateCountPair>();
		Ingredients = converterTemplate.GetRequirements();
		RequiredGems = converterTemplate.GetRequiredGems();
		RequiredTool = converterTemplate.GetTool();
		BoyReward = converterTemplate.GetBoyReward();
		GirlReward = converterTemplate.GetGirlReward();
		return true;
	}

	public static MilMo_ConverterTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_ConverterTemplate(category, path, filePath, "Converter");
	}

	protected MilMo_ConverterTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Converter(this, modifiers);
	}
}
