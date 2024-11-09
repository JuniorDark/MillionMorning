using System;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual.Materials;

public class MilMo_TransparentBumpSpec : MilMo_Material
{
	private const string SHADER_FILENAME = "Junebug/Transparent/Bumped-Specular";

	private Color _specColor = new Color(0.5f, 0.5f, 0.5f, 0f);

	private float _shininess = 5f / 64f;

	private float _specBlend = 0.25f;

	private Vector2 _uvScroll = new Vector2(0f, 0f);

	private static readonly int SpecBlend = Shader.PropertyToID("_SpecBlend");

	private static readonly int Shininess = Shader.PropertyToID("_Shininess");

	private static readonly int SpecColor = Shader.PropertyToID("_SpecColor");

	private static readonly int Color = Shader.PropertyToID("_Color");

	private static readonly int BumpMap = Shader.PropertyToID("_BumpMap");

	private static readonly int USpeed = Shader.PropertyToID("_USpeed");

	private static readonly int VSpeed = Shader.PropertyToID("_VSpeed");

	protected override string Name => "Junebug/Transparent/Bumped-Specular";

	public MilMo_TransparentBumpSpec()
		: base("Junebug/Transparent/Bumped-Specular")
	{
	}

	public override void Load(MilMo_SFFile file)
	{
		while (file.NextRow() && !file.IsNext("</MAT>"))
		{
			if (file.IsNext("MainTex"))
			{
				ReadTexture(file, "_MainTex");
			}
			else if (file.IsNext("BumpMap"))
			{
				ReadTexture(file, "_BumpMap");
			}
			else if (file.IsNext("MainColor"))
			{
				base.MainColor = file.GetColor();
			}
			else if (file.IsNext("SpecColor"))
			{
				_specColor = file.GetColor();
			}
			else if (file.IsNext("Shininess"))
			{
				_shininess = file.GetFloat();
			}
			else if (file.IsNext("SpecBlend"))
			{
				_specBlend = file.GetFloat();
			}
			else if (file.IsNext("UVScroll"))
			{
				_uvScroll = file.GetVector2();
			}
		}
	}

	protected override bool CreateInternal()
	{
		LoadAllTextures();
		base.Material.SetColor(Color, base.MainColor);
		base.Material.SetColor(SpecColor, _specColor);
		base.Material.SetFloat(Shininess, _shininess);
		base.Material.SetFloat(SpecBlend, _specBlend);
		base.Material.SetFloat(USpeed, _uvScroll.x);
		base.Material.SetFloat(VSpeed, _uvScroll.y);
		return true;
	}

	public static void Write(Material material, MilMo_SFFile file, int index, MilMo_Material template)
	{
		try
		{
			MilMo_TransparentBumpSpec milMo_TransparentBumpSpec = template as MilMo_TransparentBumpSpec;
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
				MilMo_Material.WriteTexture(material, file, milMo_TransparentBumpSpec, "_MainTex", "MainTex");
			}
			if (material.GetTexture(BumpMap) != null)
			{
				MilMo_Material.WriteTexture(material, file, milMo_TransparentBumpSpec, "_BumpMap", "BumpMap");
			}
			Color color = material.GetColor(Color);
			if (milMo_TransparentBumpSpec == null || !MilMo_Utility.Equals(color, milMo_TransparentBumpSpec.MainColor))
			{
				file.AddRow();
				file.Write("MainColor");
				file.Write(color);
			}
			Color color2 = material.GetColor(SpecColor);
			if (milMo_TransparentBumpSpec == null || !MilMo_Utility.Equals(color2, milMo_TransparentBumpSpec._specColor))
			{
				file.AddRow();
				file.Write("SpecColor");
				file.Write(color2);
			}
			float @float = material.GetFloat(Shininess);
			if (milMo_TransparentBumpSpec == null || (double)Math.Abs(@float - milMo_TransparentBumpSpec._shininess) > 0.001)
			{
				file.AddRow();
				file.Write("Shininess");
				file.Write(@float);
			}
			float float2 = material.GetFloat(SpecBlend);
			if (milMo_TransparentBumpSpec == null || (double)Math.Abs(float2 - milMo_TransparentBumpSpec._specBlend) > 0.001)
			{
				file.AddRow();
				file.Write("SpecBlend");
				file.Write(float2);
			}
			Vector2 vector = new Vector2(material.GetFloat(USpeed), material.GetFloat(VSpeed));
			if (!MilMo_Utility.Equals(vector, new Vector2(0f, 0f)) && (milMo_TransparentBumpSpec == null || !MilMo_Utility.Equals(vector, milMo_TransparentBumpSpec._uvScroll)))
			{
				file.AddRow();
				file.Write("UVScroll");
				file.Write(vector);
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
