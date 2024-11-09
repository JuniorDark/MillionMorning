using System;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual.Materials;

public class MilMo_ParticleAdditiveBlendZOffset : MilMo_Material
{
	private const string SHADER_FILENAME = "Junebug/Particles/AdditiveBlendedZOffset";

	private static readonly int TintColor = Shader.PropertyToID("_TintColor");

	protected override string Name => "Junebug/Particles/AdditiveBlendedZOffset";

	public MilMo_ParticleAdditiveBlendZOffset()
		: base("Junebug/Particles/AdditiveBlendedZOffset")
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
			MilMo_ParticleAdditiveBlendZOffset milMo_ParticleAdditiveBlendZOffset = template as MilMo_ParticleAdditiveBlendZOffset;
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
				MilMo_Material.WriteTexture(material, file, milMo_ParticleAdditiveBlendZOffset, "_MainTex", "ParticleTexture");
			}
			Color color = material.GetColor(TintColor);
			if (milMo_ParticleAdditiveBlendZOffset == null || !MilMo_Utility.Equals(color, milMo_ParticleAdditiveBlendZOffset.MainColor))
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
