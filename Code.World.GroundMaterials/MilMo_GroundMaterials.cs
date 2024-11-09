using System.Collections.Generic;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GroundMaterials;

public static class MilMo_GroundMaterials
{
	public delegate void GroundMaterialsDone(bool success);

	private static readonly Dictionary<string, string> Materials = new Dictionary<string, string>();

	public static async void AsyncLoad(string world, GroundMaterialsDone callback)
	{
		MilMo_SFFile milMo_SFFile = await MilMo_SimpleFormat.RealAsyncLoad("Content/Worlds/" + world + "/Environment/GroundMaterials");
		ReloadMaterials(milMo_SFFile);
		callback(milMo_SFFile != null);
	}

	public static void LoadInEditorMode(string world)
	{
		ReloadMaterials(MilMo_SimpleFormat.LoadLocal("Content/Worlds/" + world + "/Environment/GroundMaterials"));
	}

	public static bool IsMaterial(string material)
	{
		return Materials.ContainsKey(material);
	}

	private static void ReloadMaterials(MilMo_SFFile file)
	{
		Materials.Clear();
		if (file == null)
		{
			Debug.LogWarning("Failed to load ground materials, file is null!");
			return;
		}
		while (file.NextRow())
		{
			string @string = file.GetString();
			Materials.Add(@string, "");
		}
	}
}
