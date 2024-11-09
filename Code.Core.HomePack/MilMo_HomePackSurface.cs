using System;
using System.Collections.Generic;
using Code.Core.BodyPack;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Core;
using UnityEngine;

namespace Code.Core.HomePack;

public sealed class MilMo_HomePackSurface : MilMo_HomePackBase
{
	public delegate void HomePackSurfaceLoaded();

	private readonly MilMo_HomePackSurfacePainter _painter;

	public bool IsLoaded => _painter.IsLoaded;

	public static MilMo_Template Create(string category, string path, string filePath)
	{
		return new MilMo_HomePackSurface(category, path, filePath);
	}

	public static MilMo_HomePackSurface GetHomePackSurfaceByName(string type, string name)
	{
		MilMo_Template template = Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(type, "Homes." + name);
		if (template == null)
		{
			Debug.LogWarning("Trying to fetch non existing HomePackSurface template " + name);
			return null;
		}
		try
		{
			return (MilMo_HomePackSurface)template;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Trying to fetch a HomePackSurface with wrong template type " + name);
			Debug.LogWarning(ex.ToString());
			return null;
		}
	}

	public MilMo_HomePackSurface(string category, string path, string filePath)
		: base(category, path, filePath, "HomePackSurface")
	{
		_painter = new MilMo_HomePackSurfacePainter(path);
	}

	public override bool ReadLine(MilMo_SFFile file)
	{
		if (file.IsNext("SkinLayer"))
		{
			string @string = file.GetString();
			string colorGroupName = file.GetString();
			ColorGroup colorGroup = base.ColorGroups.Find((ColorGroup cg) => cg.GroupName.Equals(colorGroupName));
			if (colorGroup == null && !colorGroupName.Equals("NoBlend", StringComparison.InvariantCultureIgnoreCase))
			{
				Debug.LogWarning("Invalid skin layer in bodypack '" + base.Name + "'");
			}
			Vector2 uvOffset = new Vector2(0f, 0f);
			if (file.IsNext("Offset"))
			{
				uvOffset = file.GetVector2();
				uvOffset.x /= 1024f;
				uvOffset.y /= 1024f;
			}
			MilMo_TextureColorGroupPair skinLayer = new MilMo_TextureColorGroupPair(@string, colorGroup, uvOffset);
			_painter.AddSkinLayer(skinLayer);
			return true;
		}
		return base.ReadLine(file);
	}

	public void Apply(GameObject gameObject, IDictionary<string, int> colorIndices)
	{
		_painter.CreateTexture(gameObject, colorIndices);
	}

	public void AsyncLoadContent(HomePackSurfaceLoaded callback)
	{
		_painter.AsyncLoadContent(callback);
	}

	public Texture2D GetTexture(GameObject gameObject)
	{
		return _painter.GetTexture(gameObject);
	}

	public override void UnloadContent()
	{
	}
}
