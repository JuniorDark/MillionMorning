using System;
using System.Collections.Generic;
using Code.Core.BodyPack;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Core;
using UnityEngine;

namespace Code.Core.HomePack;

public sealed class MilMo_HomePack : MilMo_HomePackBase
{
	public delegate void HomePackLoaded(bool success);

	private readonly List<MilMo_HomeAddon> _addons = new List<MilMo_HomeAddon>();

	public static MilMo_Template Create(string category, string path, string filePath)
	{
		return new MilMo_HomePack(category, path, filePath);
	}

	public static MilMo_HomePack GetHomePackByName(string name)
	{
		MilMo_Template template = Singleton<MilMo_TemplateContainer>.Instance.GetTemplate("HomePack", "Homes." + name);
		if (template == null)
		{
			Debug.LogWarning("Trying to fetch non existing home pack template " + name);
			return null;
		}
		try
		{
			return (MilMo_HomePack)template;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Trying to fetch a home pack with wrong template type " + name);
			Debug.LogWarning(ex.ToString());
			return null;
		}
	}

	public MilMo_HomePack(string category, string path, string filePath)
		: base(category, path, filePath, "HomePack")
	{
	}

	public override bool ReadLine(MilMo_SFFile file)
	{
		if (file.IsNext("Addon"))
		{
			string @string = file.GetString();
			IList<MilMo_TextureColorGroupPair> list = new List<MilMo_TextureColorGroupPair>();
			while (file.HasMoreTokens())
			{
				string string2 = file.GetString();
				if (!file.HasMoreTokens())
				{
					Debug.LogWarning("Textures and color groups must be given in pairs: '" + base.Name + "'");
					return false;
				}
				string colorGroupName = file.GetString();
				ColorGroup colorGroup = base.ColorGroups.Find((ColorGroup cg) => cg.GroupName.Equals(colorGroupName));
				if (colorGroup == null && !colorGroupName.Equals("NoBlend", StringComparison.InvariantCultureIgnoreCase))
				{
					Debug.LogWarning("Invalid Addon layer in homepack '" + base.Name + "'");
				}
				Vector2 uvOffset = Vector2.zero;
				if (file.IsNext("Offset"))
				{
					uvOffset = file.GetVector2();
				}
				list.Add(new MilMo_TextureColorGroupPair(string2, colorGroup, uvOffset));
			}
			MilMo_HomeAddon item = new MilMo_HomeAddon(@string, Path, list);
			_addons.Add(item);
			return true;
		}
		return base.ReadLine(file);
	}

	public void AsyncLoadContent(GameObject gameObject, HomePackLoaded callback)
	{
		int count = _addons.Count;
		foreach (MilMo_HomeAddon addon in _addons)
		{
			MilMo_HomeAddon ad = addon;
			addon.AsyncLoadContent(gameObject, delegate(bool success)
			{
				if (success)
				{
					ad.GetGameObject(gameObject).transform.parent = gameObject.transform;
				}
				int num = count - 1;
				count = num;
				if (num <= 0)
				{
					callback(success: true);
				}
			});
		}
	}

	public void AsyncApply(GameObject gameObject, IDictionary<string, int> colorIndices)
	{
		foreach (MilMo_HomeAddon addon in _addons)
		{
			addon.CreateTexture(gameObject, colorIndices);
		}
	}

	public override void UnloadContent()
	{
		foreach (MilMo_HomeAddon addon in _addons)
		{
			addon.UnloadContent();
		}
	}

	public void UnloadContent(GameObject gameObject)
	{
		foreach (MilMo_HomeAddon addon in _addons)
		{
			addon.UnloadContent(gameObject);
		}
	}
}
