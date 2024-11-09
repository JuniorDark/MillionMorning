using System.Collections.Generic;
using Code.Core.HomePack;
using Code.Core.Items.Home.HomeEquipmentTemplate;
using UnityEngine;

namespace Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment;

public abstract class MilMo_HomeSurface : MilMo_ColoredHomeEquipment
{
	public MilMo_HomePackSurface HomePackSurface => base.HomePackBase as MilMo_HomePackSurface;

	protected MilMo_HomeSurface(MilMo_HomeSurfaceTemplate template, Dictionary<string, string> modifiers, string type)
		: base(template, modifiers, MilMo_HomePackSurface.GetHomePackSurfaceByName(type, template.HomeSurface))
	{
	}

	public void Apply(GameObject gameObject)
	{
		HomePackSurface.Apply(gameObject, base.ColorIndices);
	}

	public void AsyncLoadContent(MilMo_HomePackSurface.HomePackSurfaceLoaded callback)
	{
		HomePackSurface.AsyncLoadContent(callback);
	}

	public Texture2D GetTexture(GameObject gameObject)
	{
		return HomePackSurface.GetTexture(gameObject);
	}
}
