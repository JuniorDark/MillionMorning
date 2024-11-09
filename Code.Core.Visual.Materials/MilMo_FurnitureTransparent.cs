using System;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual.Materials;

public class MilMo_FurnitureTransparent : MilMo_Material
{
	private const string SHADER_FILENAME = "Junebug/FurnitureTransparent";

	private string _velvetRamp = "";

	private Vector4 _velvetChannel = new Vector4(0f, 1f, 0f, 0f);

	private Color _velvetColor = new Color(1f, 1f, 1f, 1f);

	private float _globalAlpha = 1f;

	private static readonly int Color = Shader.PropertyToID("_Color");

	private static readonly int VelvetColor = Shader.PropertyToID("_VelvetColor");

	private static readonly int VelvetChannel = Shader.PropertyToID("_VelvetChannel");

	private static readonly int Alpha = Shader.PropertyToID("_Alpha");

	private static readonly int Ramp1 = Shader.PropertyToID("_Ramp");

	protected override string Name => "Junebug/FurnitureTransparent";

	public MilMo_FurnitureTransparent()
		: base("Junebug/FurnitureTransparent")
	{
	}

	public override void Load(MilMo_SFFile file)
	{
		while (file.NextRow() && !file.IsNext("</MAT>"))
		{
			if (file.IsNext("Diffuse"))
			{
				ReadTexture(file, "_MainTex");
			}
			else if (file.IsNext("VelvetRamp"))
			{
				_velvetRamp = file.GetString();
			}
			else if (file.IsNext("VelvetChannel"))
			{
				_velvetChannel = file.GetVector3();
			}
			else if (file.IsNext("VelvetColor"))
			{
				_velvetColor = file.GetColor();
			}
			else if (file.IsNext("MainColor"))
			{
				base.MainColor = file.GetColor();
			}
			else if (file.IsNext("GlobalAlpha"))
			{
				_globalAlpha = file.GetFloat();
			}
		}
	}

	protected override bool CreateInternal()
	{
		LoadAllTextures(_velvetRamp);
		base.Material.SetColor(Color, base.MainColor);
		base.Material.SetColor(VelvetColor, _velvetColor);
		base.Material.SetVector(VelvetChannel, _velvetChannel);
		base.Material.SetFloat(Alpha, _globalAlpha);
		return true;
	}

	public static void Write(Material material, MilMo_SFFile file, int index, MilMo_Material template)
	{
		try
		{
			MilMo_FurnitureTransparent milMo_FurnitureTransparent = template as MilMo_FurnitureTransparent;
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
				MilMo_Material.WriteTexture(material, file, milMo_FurnitureTransparent, "_MainTex", "Diffuse");
			}
			Texture texture = material.GetTexture(Ramp1);
			if ((bool)texture && texture.name != "VelvetRamp")
			{
				string text = MilMo_ResourceManager.Instance.ResolveTexturePath(texture.name);
				if (milMo_FurnitureTransparent == null || text != milMo_FurnitureTransparent._velvetRamp)
				{
					file.AddRow();
					file.Write("VelvetRamp");
					file.Write(text);
				}
			}
			Color color = material.GetColor(Color);
			if (milMo_FurnitureTransparent == null || !MilMo_Utility.Equals(color, milMo_FurnitureTransparent.MainColor))
			{
				file.AddRow();
				file.Write("MainColor");
				file.Write(color);
			}
			Color color2 = material.GetColor(VelvetColor);
			if (milMo_FurnitureTransparent == null || !MilMo_Utility.Equals(color2, milMo_FurnitureTransparent._velvetColor))
			{
				file.AddRow();
				file.Write("VelvetColor");
				file.Write(color2);
			}
			Vector4 vector = material.GetVector(VelvetChannel);
			if (milMo_FurnitureTransparent == null || !MilMo_Utility.Equals(vector, milMo_FurnitureTransparent._velvetChannel))
			{
				file.AddRow();
				file.Write("VelvetChannel");
				file.Write(vector);
			}
			float @float = material.GetFloat(Alpha);
			file.AddRow();
			file.Write("GlobalAlpha");
			file.Write(@float);
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
