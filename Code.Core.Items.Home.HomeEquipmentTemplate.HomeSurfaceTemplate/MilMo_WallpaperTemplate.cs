using System.Collections.Generic;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.HomeSurface;

namespace Code.Core.Items.Home.HomeEquipmentTemplate.HomeSurfaceTemplate;

public sealed class MilMo_WallpaperTemplate : MilMo_HomeSurfaceTemplate
{
	public override bool IsSkin => true;

	private MilMo_WallpaperTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public static MilMo_WallpaperTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_WallpaperTemplate(category, path, filePath, "Wallpaper");
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Wallpaper(this, modifiers);
	}
}
