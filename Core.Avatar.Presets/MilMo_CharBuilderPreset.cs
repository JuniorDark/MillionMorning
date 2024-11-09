using System;
using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.Items;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using UnityEngine;

namespace Core.Avatar.Presets;

public class MilMo_CharBuilderPreset : MilMo_Template
{
	private Item _hairData;

	private Item _shoesData;

	private Item _pantsData;

	private Item _shirtData;

	private readonly List<Item> _itemsCoolData = new List<Item>();

	private MilMo_EventSystem.MilMo_Callback _wearablesDoneCallback;

	private bool _waitForCoolItems;

	public sbyte Gender { get; private set; }

	public string SkinColor { get; private set; }

	public int HairColor { get; private set; }

	public string EyeColor { get; private set; }

	public string Mouth { get; private set; }

	public string Eyes { get; private set; }

	public string EyeBrows { get; private set; }

	public float Height { get; private set; }

	public MilMo_Wearable Hair { get; private set; }

	public MilMo_Wearable Shoes { get; private set; }

	public MilMo_Wearable Pants { get; private set; }

	public MilMo_Wearable Shirt { get; private set; }

	public List<MilMo_Wearable> ItemsCool { get; private set; }

	public List<Item> UncoolItemsForNetwork => new List<Item> { _shirtData, _hairData, _shoesData, _pantsData };

	private MilMo_CharBuilderPreset(string category, string path, string filePath)
		: base(category, path, filePath, "CharBuilderPreset")
	{
		ItemsCool = new List<MilMo_Wearable>();
		HairColor = -1;
		Gender = -1;
	}

	public override bool ReadLine(MilMo_SFFile file)
	{
		if (file.IsNext("Gender"))
		{
			if (file.IsNext("Male") || file.IsNext("Boy"))
			{
				Gender = 0;
			}
			else
			{
				if (!file.IsNext("Female") && !file.IsNext("Girl"))
				{
					Debug.LogWarning("Invalid gender " + file.GetString() + "in charbuilder preset " + file.Path);
					return false;
				}
				Gender = 1;
			}
		}
		else if (file.IsNext("SkinColor"))
		{
			SkinColor = file.GetString();
		}
		else if (file.IsNext("HairColor"))
		{
			HairColor = file.GetInt();
		}
		else if (file.IsNext("EyeColor"))
		{
			EyeColor = file.GetString();
		}
		else if (file.IsNext("Mouth"))
		{
			Mouth = file.GetString();
		}
		else if (file.IsNext("Eyes"))
		{
			Eyes = file.GetString();
		}
		else if (file.IsNext("EyeBrows"))
		{
			EyeBrows = file.GetString();
		}
		else if (file.IsNext("Height"))
		{
			Height = file.GetFloat();
		}
		else if (file.IsNext("<HAIR>"))
		{
			file.NextRow();
			if (!file.IsNext("<ITEM>"))
			{
				Debug.LogWarning("Missing <ITEM> tag at line " + file.GetLineNumber() + " of " + file.Path + " (next token is " + file.GetString() + ")");
				return false;
			}
			_hairData = ReadItem(file);
			file.NextRow();
			if (!file.IsNext("</HAIR>"))
			{
				return false;
			}
		}
		else if (file.IsNext("<SHOES>"))
		{
			file.NextRow();
			if (!file.IsNext("<ITEM>"))
			{
				Debug.LogWarning("Missing <ITEM> tag at line " + file.GetLineNumber() + " of " + file.Path + " (next token is " + file.GetString() + ")");
				return false;
			}
			_shoesData = ReadItem(file);
			file.NextRow();
			if (!file.IsNext("</SHOES>"))
			{
				return false;
			}
		}
		else if (file.IsNext("<SHIRT>"))
		{
			file.NextRow();
			if (!file.IsNext("<ITEM>"))
			{
				Debug.LogWarning("Missing <ITEM> tag at line " + file.GetLineNumber() + " of " + file.Path + " (next token is " + file.GetString() + ")");
				return false;
			}
			_shirtData = ReadItem(file);
			file.NextRow();
			if (!file.IsNext("</SHIRT>"))
			{
				return false;
			}
		}
		else if (file.IsNext("<PANTS>"))
		{
			file.NextRow();
			if (!file.IsNext("<ITEM>"))
			{
				Debug.LogWarning("Missing <ITEM> tag at line " + file.GetLineNumber() + " of " + file.Path + " (next token is " + file.GetString() + ")");
				return false;
			}
			_pantsData = ReadItem(file);
			file.NextRow();
			if (!file.IsNext("</PANTS>"))
			{
				return false;
			}
		}
		else
		{
			if (!file.IsNext("<COOL>"))
			{
				return base.ReadLine(file);
			}
			while (file.NextRow())
			{
				if (file.IsNext("<ITEM>"))
				{
					Item item = ReadItem(file);
					if (item == null)
					{
						return false;
					}
					_itemsCoolData.Add(item);
					continue;
				}
				if (file.IsNext("</COOL>"))
				{
					break;
				}
				Debug.LogWarning("Unknown parameter " + file.GetString() + " in cool section of " + file.Path + " (it will be ignored).");
			}
		}
		return true;
	}

	public override bool FinishLoading()
	{
		if (Gender != -1 && !string.IsNullOrEmpty(SkinColor) && HairColor != -1 && !string.IsNullOrEmpty(EyeColor) && !string.IsNullOrEmpty(Mouth) && !string.IsNullOrEmpty(Eyes) && !string.IsNullOrEmpty(EyeBrows) && !((double)Math.Abs(Height) < 0.0) && _hairData != null && _shoesData != null && _pantsData != null && _shirtData != null)
		{
			return base.FinishLoading();
		}
		if (Gender == -1)
		{
			Debug.LogWarning("Gender not set in " + Category + ":" + Path);
		}
		if (string.IsNullOrEmpty(SkinColor))
		{
			Debug.LogWarning("SkinColor not set in " + Category + ":" + Path);
		}
		if (HairColor == -1)
		{
			Debug.LogWarning("HairColor not set in " + Category + ":" + Path);
		}
		if (string.IsNullOrEmpty(EyeColor))
		{
			Debug.LogWarning("EyeColor not set in " + Category + ":" + Path);
		}
		if (string.IsNullOrEmpty(Mouth))
		{
			Debug.LogWarning("Mouth not set in " + Category + ":" + Path);
		}
		if (string.IsNullOrEmpty(Eyes))
		{
			Debug.LogWarning("Eyes not set in " + Category + ":" + Path);
		}
		if (string.IsNullOrEmpty(EyeBrows))
		{
			Debug.LogWarning("Mouth not set in " + Category + ":" + Path);
		}
		if ((double)Math.Abs(Height) < 0.0)
		{
			Debug.LogWarning("Height not set in " + Category + ":" + Path);
		}
		if (_hairData == null)
		{
			Debug.LogWarning("Hair not set in " + Category + ":" + Path);
		}
		if (_shoesData == null)
		{
			Debug.LogWarning("Shoes not set in " + Category + ":" + Path);
		}
		if (_pantsData == null)
		{
			Debug.LogWarning("Pants not set in " + Category + ":" + Path);
		}
		if (_shirtData == null)
		{
			Debug.LogWarning("Shirt not set in " + Category + ":" + Path);
		}
		return false;
	}

	public void AsyncLoadWearables(bool loadCoolItems, MilMo_EventSystem.MilMo_Callback wearablesDoneCallback)
	{
		_wearablesDoneCallback = wearablesDoneCallback;
		_waitForCoolItems = loadCoolItems;
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(_hairData.GetTemplate(), WearableTemplateArrived);
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(_shoesData.GetTemplate(), WearableTemplateArrived);
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(_shirtData.GetTemplate(), WearableTemplateArrived);
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(_pantsData.GetTemplate(), WearableTemplateArrived);
		if (!loadCoolItems)
		{
			return;
		}
		foreach (Item itemsCoolDatum in _itemsCoolData)
		{
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(itemsCoolDatum.GetTemplate(), WearableTemplateArrived);
		}
	}

	private void WearableTemplateArrived(MilMo_Template template, bool timeout)
	{
		MilMo_WearableTemplate milMo_WearableTemplate = template as MilMo_WearableTemplate;
		if (timeout || milMo_WearableTemplate == null)
		{
			return;
		}
		if (_hairData.GetTemplate().GetCategory() == template.Category && _hairData.GetTemplate().GetPath() == template.Path)
		{
			Hair = (MilMo_Wearable)milMo_WearableTemplate.Instantiate(MilMo_Item.ReadModifiers(_hairData.GetModifiers()));
		}
		if (_shoesData.GetTemplate().GetCategory() == template.Category && _shoesData.GetTemplate().GetPath() == template.Path)
		{
			Shoes = (MilMo_Wearable)milMo_WearableTemplate.Instantiate(MilMo_Item.ReadModifiers(_shoesData.GetModifiers()));
		}
		if (_shirtData.GetTemplate().GetCategory() == template.Category && _shirtData.GetTemplate().GetPath() == template.Path)
		{
			Shirt = (MilMo_Wearable)milMo_WearableTemplate.Instantiate(MilMo_Item.ReadModifiers(_shirtData.GetModifiers()));
		}
		if (_pantsData.GetTemplate().GetCategory() == template.Category && _pantsData.GetTemplate().GetPath() == template.Path)
		{
			Pants = (MilMo_Wearable)milMo_WearableTemplate.Instantiate(MilMo_Item.ReadModifiers(_pantsData.GetModifiers()));
		}
		if (_waitForCoolItems)
		{
			foreach (Item itemsCoolDatum in _itemsCoolData)
			{
				if (itemsCoolDatum.GetTemplate().GetCategory() == template.Category && itemsCoolDatum.GetTemplate().GetPath() == template.Path)
				{
					ItemsCool.Add((MilMo_Wearable)milMo_WearableTemplate.Instantiate(MilMo_Item.ReadModifiers(itemsCoolDatum.GetModifiers())));
				}
			}
		}
		if (_wearablesDoneCallback != null && Hair != null && Shoes != null && Shirt != null && Pants != null && (!_waitForCoolItems || ItemsCool.Count == _itemsCoolData.Count))
		{
			_wearablesDoneCallback();
		}
	}

	private static Item ReadItem(MilMo_SFFile file)
	{
		List<string> list = new List<string>();
		TemplateReference templateReference = null;
		while (file.NextRow())
		{
			if (file.IsNext("</ITEM>"))
			{
				if (templateReference != null)
				{
					return new Item(templateReference, list);
				}
				return null;
			}
			if (file.IsNext("Identifier"))
			{
				try
				{
					string[] array = file.GetString().Split(':');
					templateReference = new TemplateReference(array[0], array[1]);
				}
				catch (IndexOutOfRangeException)
				{
					Debug.LogWarning("Invalid item identifier in charbuilder preset " + file.Path);
					return null;
				}
			}
			else if (file.IsNext("Modifier"))
			{
				list.Add(file.GetString());
			}
		}
		Debug.LogWarning("Missing </ITEM> end tag in charbuilder preset " + file.Path);
		return null;
	}

	public static MilMo_CharBuilderPreset Create(string category, string path, string filePath)
	{
		return new MilMo_CharBuilderPreset(category, path, filePath);
	}
}
