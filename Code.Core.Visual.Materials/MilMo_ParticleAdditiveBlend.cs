using System;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual.Materials;

public class MilMo_ParticleAdditiveBlend : MilMo_Material
{
	private const string SHADER_FILENAME = "Junebug/Particles/AdditiveBlended";

	private static readonly int TintColor = Shader.PropertyToID("_TintColor");

	protected override string Name => "Junebug/Particles/AdditiveBlended";

	public MilMo_ParticleAdditiveBlend()
		: base("Junebug/Particles/AdditiveBlended")
	{
		MainColorName = "_TintColor";
	}

	public override void Load(MilMo_SFFile file)
	{
		while (file.NextRow() && !file.IsNext("</MAT>"))
		{
			if (file.IsNext("ParticleTexture"))
			{
				ReadTexture(file, "_MainTex");
			}
			else if (file.IsNext("TintColor"))
			{
				base.MainColor = file.GetColor();
			}
		}
	}

	protected override bool CreateInternal()
	{
		LoadAllTextures();
		base.Material.SetColor(TintColor, base.MainColor);
		return true;
	}

	public static void Write(Material material, MilMo_SFFile file, int index, MilMo_Material template)
	{
		try
		{
			MilMo_ParticleAdditiveBlend milMo_ParticleAdditiveBlend = template as MilMo_ParticleAdditiveBlend;
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
			if (material.mainTexture != null)
			{
				MilMo_Material.WriteTexture(material, file, milMo_ParticleAdditiveBlend, "_MainTex", "ParticleTexture");
			}
			Color color = material.GetColor(TintColor);
			if (milMo_ParticleAdditiveBlend == null || !MilMo_Utility.Equals(color, milMo_ParticleAdditiveBlend.MainColor))
			{
				file.AddRow();
				file.Write("TintColor");
				file.Write(color);
			}
			file.AddRow();
			file.Write("</MAT>");
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Failed to write " + material.shader.name + " material");
			Debug.LogWarning(ex.ToString());
		}
	}
}
