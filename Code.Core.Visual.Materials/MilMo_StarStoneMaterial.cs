using System;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual.Materials;

public class MilMo_StarStoneMaterial : MilMo_Material
{
	private const string SHADER_FILENAME = "Junebug/StarStoneShader";

	private Color _amb = new Color(0.1f, 0f, 0f, 0.1f);

	private Color _spec = new Color(1f, 1f, 1f, 1f);

	private float _shin = 0.3f;

	private float _texStrength = 0.3f;

	private string _velvetRamp = "";

	private Vector4 _velvetChannel = new Vector4(1f, 0f, 0f, 0f);

	private static readonly int Color = Shader.PropertyToID("_Color");

	private static readonly int AmbColor = Shader.PropertyToID("_AmbColor");

	private static readonly int SpecularColor = Shader.PropertyToID("_SpecularColor");

	private static readonly int Shininess = Shader.PropertyToID("_Shininess");

	private static readonly int TextureStrength = Shader.PropertyToID("_TextureStrength");

	private static readonly int VelvetChannel = Shader.PropertyToID("_VelvetChannel");

	private static readonly int Ramp1 = Shader.PropertyToID("_Ramp");

	protected override string Name => "Junebug/StarStoneShader";

	public MilMo_StarStoneMaterial()
		: base("Junebug/StarStoneShader")
	{
	}

	public override void Load(MilMo_SFFile file)
	{
		while (file.NextRow() && !file.IsNext("</MAT>"))
		{
			if (file.IsNext("MainColor"))
			{
				base.MainColor = file.GetColor();
			}
			else if (file.IsNext("Diffuse"))
			{
				ReadTexture(file, "_StructureTex");
			}
			else if (file.IsNext("AmbientColor"))
			{
				_amb = file.GetColor();
			}
			else if (file.IsNext("SpecularColor"))
			{
				_spec = file.GetColor();
			}
			else if (file.IsNext("Shininess"))
			{
				_shin = file.GetFloat();
			}
			else if (file.IsNext("TextureStrength"))
			{
				_texStrength = file.GetFloat();
			}
			else if (file.IsNext("VelvetRamp"))
			{
				_velvetRamp = file.GetString();
			}
			else if (file.IsNext("VelvetChannel"))
			{
				_velvetChannel = file.GetVector3();
			}
		}
	}

	protected override bool CreateInternal()
	{
		LoadAllTextures(_velvetRamp);
		base.Material.SetColor(Color, base.MainColor);
		base.Material.SetColor(AmbColor, _amb);
		base.Material.SetColor(SpecularColor, _spec);
		base.Material.SetFloat(Shininess, _shin);
		base.Material.SetFloat(TextureStrength, _texStrength);
		base.Material.SetVector(VelvetChannel, _velvetChannel);
		return true;
	}

	public static void Write(Material material, MilMo_SFFile file, int index, MilMo_Material template)
	{
		try
		{
			MilMo_StarStoneMaterial milMo_StarStoneMaterial = template as MilMo_StarStoneMaterial;
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
				MilMo_Material.WriteTexture(material, file, milMo_StarStoneMaterial, "_StructureTex", "Diffuse");
			}
			Texture texture = material.GetTexture(Ramp1);
			if ((bool)texture && texture.name != "VelvetRamp")
			{
				string text = MilMo_ResourceManager.Instance.ResolveTexturePath(texture.name);
				if (milMo_StarStoneMaterial == null || text != milMo_StarStoneMaterial._velvetRamp)
				{
					file.AddRow();
					file.Write("VelvetRamp");
					file.Write(text);
				}
			}
			Color color = material.GetColor(Color);
			if (milMo_StarStoneMaterial == null || !MilMo_Utility.Equals(color, milMo_StarStoneMaterial.MainColor))
			{
				file.AddRow();
				file.Write("MainColor");
				file.Write(color);
			}
			Color color2 = material.GetColor(AmbColor);
			if (milMo_StarStoneMaterial == null || !MilMo_Utility.Equals(color2, milMo_StarStoneMaterial._amb))
			{
				file.AddRow();
				file.Write("AmbientColor");
				file.Write(color2);
			}
			Color color3 = material.GetColor(SpecularColor);
			if (milMo_StarStoneMaterial == null || !MilMo_Utility.Equals(color3, milMo_StarStoneMaterial._spec))
			{
				file.AddRow();
				file.Write("SpecularColor");
				file.Write(color3);
			}
			float @float = material.GetFloat(Shininess);
			if (milMo_StarStoneMaterial == null || !object.Equals(@float, milMo_StarStoneMaterial._shin))
			{
				file.AddRow();
				file.Write("Shininess");
				file.Write(@float);
			}
			Vector4 vector = material.GetVector(VelvetChannel);
			if (milMo_StarStoneMaterial == null || !MilMo_Utility.Equals(vector, milMo_StarStoneMaterial._velvetChannel))
			{
				file.AddRow();
				file.Write("VelvetChannel");
				file.Write(vector);
			}
			float float2 = material.GetFloat(TextureStrength);
			if (milMo_StarStoneMaterial == null || !object.Equals(float2, milMo_StarStoneMaterial._texStrength))
			{
				file.AddRow();
				file.Write("TextureStrength");
				file.Write(float2);
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
