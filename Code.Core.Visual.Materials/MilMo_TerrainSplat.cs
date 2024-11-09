using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Visual.Materials;

public class MilMo_TerrainSplat : MilMo_Material
{
	private const string SHADER_FILENAME = "Junebug/TerrainSplat";

	private static readonly int Control = Shader.PropertyToID("_Control");

	private static readonly int Splat0 = Shader.PropertyToID("_Splat0");

	private static readonly int Splat1 = Shader.PropertyToID("_Splat1");

	private static readonly int Splat2 = Shader.PropertyToID("_Splat2");

	private static readonly int Splat3 = Shader.PropertyToID("_Splat3");

	protected override string Name => "Junebug/TerrainSplat";

	public MilMo_TerrainSplat()
		: base("Junebug/TerrainSplat")
	{
	}

	public override void Load(MilMo_SFFile file)
	{
		while (file.NextRow() && !file.IsNext("</MAT>"))
		{
			if (file.IsNext("Control"))
			{
				ReadTexture(file, "_Control");
			}
			else if (file.IsNext("Red"))
			{
				ReadTexture(file, "_Splat0");
			}
			else if (file.IsNext("Green"))
			{
				ReadTexture(file, "_Splat1");
			}
			else if (file.IsNext("Blue"))
			{
				ReadTexture(file, "_Splat2");
			}
			else if (file.IsNext("Alpha"))
			{
				ReadTexture(file, "_Splat3");
			}
		}
	}

	protected override bool CreateInternal()
	{
		LoadAllTextures();
		return true;
	}

	public static void Write(Material material, MilMo_SFFile file, int index, MilMo_Material template)
	{
		MilMo_TerrainSplat template2 = template as MilMo_TerrainSplat;
		file.AddRow();
		file.Write("<MAT>");
		if (index != -1)
		{
			file.AddRow();
			file.Write("Index");
			file.Write(index);
		}
		file.AddRow();
		file.Write(material.shader.name);
		if ((bool)material.GetTexture(Control))
		{
			MilMo_Material.WriteTexture(material, file, template2, "_Control", "Control");
		}
		if ((bool)material.GetTexture(Splat0))
		{
			MilMo_Material.WriteTexture(material, file, template2, "_Splat0", "Red");
		}
		if ((bool)material.GetTexture(Splat1))
		{
			MilMo_Material.WriteTexture(material, file, template2, "_Splat1", "Green");
		}
		if ((bool)material.GetTexture(Splat2))
		{
			MilMo_Material.WriteTexture(material, file, template2, "_Splat2", "Blue");
		}
		if ((bool)material.GetTexture(Splat3))
		{
			MilMo_Material.WriteTexture(material, file, template2, "_Splat3", "Alpha");
		}
		file.AddRow();
		file.Write("</MAT>");
	}
}
