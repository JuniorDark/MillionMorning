using System.Collections.Generic;
using Code.Core.BodyPack;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.HomePack;

public abstract class MilMo_HomePackBase : MilMo_Template
{
	private List<ColorGroup> m_ColorGroups = new List<ColorGroup>();

	private IDictionary<string, IList<int>> m_DefaultColorIndices = new Dictionary<string, IList<int>>();

	public List<ColorGroup> ColorGroups => m_ColorGroups;

	public MilMo_HomePackBase(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override bool ReadLine(MilMo_SFFile file)
	{
		if (file.IsNext("ColorGroup"))
		{
			string @string = file.GetString();
			List<int> list = new List<int>();
			IDictionary<int, int> dictionary = new Dictionary<int, int>();
			while (file.HasMoreTokens())
			{
				string string2 = file.GetString();
				if (int.TryParse(string2, out var result))
				{
					if (MilMo_BodyPackSystem.GetColorFromIndex(result) == null)
					{
						Debug.LogWarning("Invalid Color Index " + string2);
					}
					else if (!dictionary.ContainsKey(result))
					{
						list.Add(result);
						dictionary.Add(result, result);
					}
					continue;
				}
				IList<int> templateColorIndices = MilMo_BodyPackSystem.GetTemplateColorIndices(string2);
				if (templateColorIndices == null)
				{
					Debug.LogWarning("Invalid ColorTemplate " + string2);
					continue;
				}
				foreach (int item in templateColorIndices)
				{
					if (!dictionary.ContainsKey(item))
					{
						list.Add(item);
						dictionary.Add(item, item);
					}
				}
			}
			m_ColorGroups.Add(new ColorGroup(@string, list));
		}
		else
		{
			if (!file.IsNext("ColorGroupDefault"))
			{
				return base.ReadLine(file);
			}
			string colorGroupName = file.GetString();
			ColorGroup colorGroup = ColorGroups.Find((ColorGroup cg) => cg.GroupName.Equals(colorGroupName));
			if (colorGroup != null)
			{
				List<int> list2 = new List<int>();
				while (file.HasMoreTokens())
				{
					int @int = file.GetInt();
					if (colorGroup.ColorIndices.Contains(@int))
					{
						list2.Add(@int);
					}
					else
					{
						Debug.LogWarning("Invalid default color in HomePack " + base.Name);
					}
				}
				m_DefaultColorIndices.Add(colorGroupName, list2);
			}
			else
			{
				Debug.LogWarning("Non existing ColorGroup " + colorGroupName + " for ColorGroupDefault in HomePack " + base.Name);
			}
		}
		return true;
	}

	public int GetDefaultColorIndex(ColorGroup colorGroup)
	{
		if (colorGroup == null)
		{
			return -1;
		}
		if (m_DefaultColorIndices.ContainsKey(colorGroup.GroupName))
		{
			IList<int> list = m_DefaultColorIndices[colorGroup.GroupName];
			switch (list.Count)
			{
			case 1:
				return list[0];
			default:
				return list[MilMo_Utility.RandomInt(0, list.Count - 1)];
			case 0:
				break;
			}
		}
		if (colorGroup.ColorIndices == null || colorGroup.ColorIndices.Count == 0)
		{
			return -1;
		}
		return colorGroup.ColorIndices[MilMo_Utility.RandomInt(0, colorGroup.ColorIndices.Count - 1)];
	}

	public abstract void UnloadContent();
}
