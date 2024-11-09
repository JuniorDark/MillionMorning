using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Visual.Materials;

public class MilMo_CutOut : MilMo_Material
{
	private const string SHADER_FILENAME = "Junebug/CutOut";

	protected override string Name => "Junebug/CutOut";

	public MilMo_CutOut()
		: base("Junebug/CutOut")
	{
	}

	public override void Load(MilMo_SFFile file)
	{
		while (file.NextRow() && !file.IsNext("</MAT>"))
		{
		}
	}

	protected override bool CreateInternal()
	{
		LoadAllTextures();
		return true;
	}

	public static void Write(Material material, MilMo_SFFile file, int index, MilMo_Material template)
	{
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
		file.AddRow();
		file.Write("</MAT>");
	}
}
